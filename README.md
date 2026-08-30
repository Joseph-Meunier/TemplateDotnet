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

