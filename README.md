# ASP.NET Core SaaS Template

Template de backend SaaS basé sur **ASP.NET Core** et **.NET 10**. Il fournit une base réutilisable pour démarrer rapidement une application moderne, modulaire et prête à être déployée.

## Fonctionnalités

- Architecture de monolithe modulaire avec Vertical Slice
- PostgreSQL et Entity Framework Core
- Authentification JWT via un fournisseur OpenID Connect, tel que Keycloak
- RabbitMQ pour les événements d’intégration
- Patterns Outbox et Inbox
- Health checks, OpenTelemetry et limitation de débit
- Tests d’intégration avec Testcontainers
- Docker Compose et CI locale

## Stack technique

- .NET 10 et ASP.NET Core Minimal API
- Entity Framework Core et PostgreSQL
- FluentValidation
- Keycloak / JWT Bearer
- RabbitMQ
- OpenAPI et Scalar
- OpenTelemetry
- xUnit et Testcontainers
- Docker Compose

## Architecture

Le projet est organisé en **monolithe modulaire**. Chaque module possède ses propres éléments de persistance et de domaine :

- un `DbContext` ;
- un schéma PostgreSQL ;
- ses migrations EF Core ;
- ses entités et fonctionnalités ;
- ses contrats publics.

Les modules ne partagent pas directement leurs entités EF Core et n'utilisent pas de clés étrangères entre eux.

```text
Acme.Api
Acme.Shared
Acme.Modules.Users
Acme.Modules.Blog
Acme.Api.IntegrationTests
```

## Créer un projet

Installez le template depuis ce dépôt :

```bash
dotnet new install .
```

Créez ensuite une application :

```bash
dotnet new saas-template -n Acme
cd Acme
```

## Configuration locale

Créez votre fichier d'environnement à partir de l'exemple :

```bash
cp .env.example .env
```

Adaptez les valeurs selon votre environnement. Les secrets et les valeurs personnelles ne doivent jamais être ajoutés à Git.

Pour des réglages propres à une machine locale, vous pouvez aussi créer `Template.Api/appsettings.Local.json`. Ce fichier est ignoré par Git.

### Principales variables d'environnement

| Groupe | Variables |
| --- | --- |
| PostgreSQL | `POSTGRES_DB`, `POSTGRES_USER`, `POSTGRES_PASSWORD`, `POSTGRES_PORT` |
| RabbitMQ | `RABBITMQ_USER`, `RABBITMQ_PASSWORD`, `RABBITMQ_PORT`, `RABBITMQ_MANAGEMENT_PORT` |
| Authentification | `AUTHENTICATION_AUTHORITY`, `AUTHENTICATION_AUDIENCE` |
| CORS | `CORS_ALLOWED_ORIGIN_0` |
| API | `API_IMAGE`, `API_PORT` |

## Démarrer avec Docker

```bash
docker compose up --build
```

Les services suivants sont alors disponibles :

| Service | Adresse par défaut |
| --- | --- |
| API | <http://localhost:8080> |
| PostgreSQL | `localhost:5432` |
| RabbitMQ | `localhost:5672` |
| Interface RabbitMQ | <http://localhost:15672> |

Les ports peuvent être modifiés dans le fichier `.env`.

## Documentation de l'API

En environnement `Development`, la référence OpenAPI est exposée via Scalar. L'endpoint est généralement disponible à l'adresse :

```text
/scalar/v1
```

## Health checks

| Endpoint | Rôle |
| --- | --- |
| `GET /health/live` | Vérifie que l'API est en cours d'exécution. |
| `GET /health/ready` | Vérifie notamment PostgreSQL et RabbitMQ. |

## Base de données et migrations

Chaque module possède son propre `DbContext` et son propre schéma PostgreSQL.

```text
UsersDbContext → schéma users
BlogDbContext  → schéma blog
```

Les migrations sont appliquées automatiquement au démarrage en environnement `Production`. En `Development`, elles restent explicites.

## Authentification

L'API utilise JWT Bearer avec un fournisseur OpenID Connect compatible, prévu initialement pour Keycloak.

Exemple de configuration :

```json
{
  "Authentication": {
    "Authority": "https://keycloak.example.com/realms/acme",
    "Audience": "acme-api"
  }
}
```

En production, fournissez ces valeurs via des variables d'environnement.

## Messagerie

RabbitMQ est utilisé pour les événements d'intégration. Le template comprend :

- un publisher et des consumers ;
- la configuration de la topologie RabbitMQ ;
- le pattern Outbox ;
- le pattern Inbox ;
- des tentatives de republication côté Outbox ;
- une Dead Letter Queue côté consommation.

L'Outbox garantit qu'un événement n'est publié qu'après la persistance de la transaction métier. L'Inbox évite le traitement répété d'un même message.

## Organisation des fonctionnalités

Les fonctionnalités suivent une organisation Vertical Slice :

```text
Features/
└── CreatePost/
    ├── Endpoint.cs
    ├── Handler.cs
    ├── Request.cs
    ├── Response.cs
    └── Validator.cs
```

Une fonctionnalité ne contient que les fichiers dont elle a réellement besoin.

## Ajouter un nouveau module

Chaque module métier est isolé dans son propre projet.

```text
Acme.Modules.Catalog/
├── Contracts/
├── Data/
├── Domain/
├── Features/
└── Migrations/
```

### 1. Créer le projet

Depuis la racine de la solution :

```bash
dotnet new classlib -n Acme.Modules.Catalog
dotnet sln add Acme.Modules.Catalog/Acme.Modules.Catalog.csproj
dotnet add Acme.Modules.Catalog reference Acme.Shared
```

Si l'API doit enregistrer ou exposer le module, ajoutez également sa référence :

```bash
dotnet add Acme.Api reference Acme.Modules.Catalog
```

### 2. Ajouter le `DbContext` du module

```csharp
public class CatalogDbContext(
    DbContextOptions<CatalogDbContext> options)
    : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");

        base.OnModelCreating(modelBuilder);
    }
}
```

Chaque module possède son propre `DbContext` et son propre schéma PostgreSQL. Ne créez pas de clé étrangère EF Core entre deux modules.

Lorsqu'un module a besoin de données d'un autre module, utilisez un contrat public :

```text
Catalog
   │
   └── IUserReader
           │
           └── Users
```

### 3. Enregistrer le module

Ajoutez le `DbContext`, les handlers et les éventuels services du module à la configuration de l'application.

```csharp
builder.Services.AddDbContext<CatalogDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("CatalogDatabase"));
});
```

Ajoutez la chaîne de connexion correspondante :

```json
{
  "ConnectionStrings": {
    "CatalogDatabase": "Host=localhost;Port=5432;Database=acme;Username=postgres;Password=change-me"
  }
}
```

Et, dans Docker Compose :

```yaml
ConnectionStrings__CatalogDatabase: Host=postgres;Port=5432;Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}
```

### 4. Créer la migration initiale

Une fois le modèle du module prêt, créez sa première migration. Consultez la section [Créer et appliquer une migration EF Core](#créer-et-appliquer-une-migration-ef-core).

## Ajouter une nouvelle feature

Le template utilise une organisation Vertical Slice : chaque fonctionnalité métier est regroupée dans son propre dossier.

```text
Features/
└── CreateProduct/
    ├── Endpoint.cs
    ├── Handler.cs
    ├── Request.cs
    ├── Response.cs
    └── Validator.cs
```

Tous les fichiers ne sont pas obligatoires. Une query simple peut, par exemple, ne contenir que :

```text
GetProduct/
├── Endpoint.cs
├── Handler.cs
└── Response.cs
```

### Command

Une command modifie l'état de l'application, par exemple : `CreateProduct`, `UpdateProduct`, `DeleteProduct`, `PublishPost` ou `AddUserRole`.

Le handler contient la logique applicative de la feature :

```csharp
public class Handler(CatalogDbContext dbContext)
{
    public async Task<Guid> Handle(
        Request request,
        CancellationToken cancellationToken)
    {
        var product = new Product(
            Guid.NewGuid(),
            request.Name);

        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync(cancellationToken);

        return product.Id;
    }
}
```

### Query

Une query lit des données sans modifier l'état. Utilisez autant que possible `AsNoTracking()` et projetez directement vers le modèle de réponse :

```csharp
var product = await dbContext.Products
    .AsNoTracking()
    .Where(x => x.Id == id)
    .Select(x => new Response(
        x.Id,
        x.Name))
    .SingleOrDefaultAsync(cancellationToken);
```

Cela évite de charger inutilement les entités complètes.

### Endpoint

L'endpoint reste léger :

```text
HTTP → validation → handler → réponse HTTP
```

```csharp
public static class Endpoint
{
    public static void MapCreateProduct(this IEndpointRouteBuilder app)
    {
        app.MapPost("/catalog/products", async (
            Request request,
            Handler handler,
            CancellationToken cancellationToken) =>
        {
            var id = await handler.Handle(request, cancellationToken);

            return Results.Created(
                $"/catalog/products/{id}",
                new Response(id));
        });
    }
}
```

Évitez de placer la logique métier directement dans l'endpoint.

### Validation

Une feature qui reçoit une requête peut utiliser FluentValidation :

```csharp
public class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}
```

Le filtre de validation global du template retourne ensuite une réponse HTTP adaptée.

### Communication entre modules

Une feature ne doit pas accéder directement au `DbContext` d'un autre module.

```text
À éviter :  Catalog Handler → UsersDbContext
À préférer : Catalog Handler → IUserReader → Users module
```

Le module propriétaire des données expose le contrat public nécessaire.

## Créer et appliquer une migration EF Core

Chaque module possède ses propres migrations, car chaque module possède son propre `DbContext`.

```text
Acme.Modules.Users/
└── Migrations/

Acme.Modules.Blog/
└── Migrations/
```

### Créer une migration

Depuis la racine du projet, pour le module Users :

```bash
dotnet ef migrations add AddSomething \
  --project Acme.Modules.Users \
  --startup-project Acme.Api \
  --context UsersDbContext
```

Pour le module Blog :

```bash
dotnet ef migrations add AddSomething \
  --project Acme.Modules.Blog \
  --startup-project Acme.Api \
  --context BlogDbContext
```

- `--project` indique le projet qui contient le `DbContext` et les migrations ;
- `--startup-project` indique le projet utilisé pour charger la configuration et l'injection de dépendances ;
- `--context` est nécessaire lorsque l'application contient plusieurs `DbContext`.

### Appliquer les migrations en développement

Pour Users :

```bash
dotnet ef database update \
  --project Acme.Modules.Users \
  --startup-project Acme.Api \
  --context UsersDbContext
```

Pour Blog :

```bash
dotnet ef database update \
  --project Acme.Modules.Blog \
  --startup-project Acme.Api \
  --context BlogDbContext
```

### Production

En environnement `Production`, l'application applique automatiquement les migrations au démarrage :

```text
container start → Users migrations → Blog migrations → application start
```

Cette stratégie convient à un déploiement simple avec une seule instance de l'API. Avec plusieurs instances susceptibles de démarrer simultanément, préférez une étape de migration dédiée avant le déploiement.

### Tests

Les tests d'intégration utilisent une base PostgreSQL créée par Testcontainers :

```text
PostgreSQL container → Users migrations → Blog migrations → tests
```

Cela vérifie que toutes les migrations peuvent être appliquées sur une base vierge. Il n'est donc pas nécessaire d'exécuter `dotnet ef migrations list` dans la CI.

## Tests

Lancez tous les tests avec :

```bash
dotnet test
```

Les tests d'intégration utilisent Testcontainers, qui démarre ses propres instances PostgreSQL et RabbitMQ. Les bases de test sont créées à la volée et les migrations EF Core sont appliquées automatiquement.

## CI locale

La CI locale exécute, dans cet ordre :

```text
restore → build Release → tests
```

Pour la lancer :

```bash
docker compose -f ci/compose.yml run --rm ci
```

Testcontainers utilise le Docker Engine de la machine hôte.

## Construire l'image API

Pour construire uniquement l'image de l'API :

```bash
docker build -f Template.Api/Dockerfile -t acme-api .
```

L'image utilise un build multi-stage :

```text
.NET SDK → restore → build → publish → ASP.NET Core Runtime
```

## Principes

Le template privilégie :

- la simplicité ;
- un faible couplage ;
- l'explicite plutôt que la magie ;
- la modularité ;
- la testabilité ;
- une infrastructure réaliste ;
- l'absence de dépendance à MediatR ;
- l'absence de repository générique et de couches inutiles.
