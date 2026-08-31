# Créer un realm Keycloak

Cette procédure permet de configurer rapidement Keycloak pour une nouvelle instance du template. Elle utilise l'exemple `mon-projet` ; remplacez-le par le nom de votre projet.

## 1. Créer le realm

Dans la console d'administration Keycloak, sélectionnez **Create realm** puis renseignez :

| Paramètre | Valeur |
| --- | --- |
| Realm name | `mon-projet` |
| Enabled | `ON` |

Les URLs OpenID Connect (OIDC) du realm sont alors basées sur :

```text
https://<ton-keycloak>/realms/mon-projet
```

Sa configuration OIDC est disponible à l'adresse suivante :

```text
/realms/{realm}/.well-known/openid-configuration
```

## 2. Créer le client de l'API

Ouvrez **Clients → Create client** et créez le client suivant.

### General settings

| Paramètre | Valeur |
| --- | --- |
| Client type | `OpenID Connect` |
| Client ID | `mon-projet-api` |

Le *Client ID* identifie le client OIDC.

### Capability config

| Paramètre | Valeur |
| --- | --- |
| Client authentication | `ON` |
| Standard flow | `OFF` |
| Direct access grants | `OFF` |
| Implicit flow | `OFF` |
| Service accounts roles | `OFF` |
| OAuth 2.0 Device Authorization | `OFF` |
| OIDC CIBA Grant | `OFF` |

`mon-projet-api` représente la ressource API qui valide les tokens ; il ne sert pas à connecter directement un utilisateur depuis Scalar. Aucun *redirect URI* n'est donc requis pour ce client.

## 3. Créer l'audience de l'API

L'API .NET configure l'audience attendue ainsi :

```csharp
options.Audience = "mon-projet-api";
```

Le JWT doit donc contenir :

```json
{ "aud": "mon-projet-api" }
```

Créez un *Client Scope* dédié via **Client scopes → Create client scope** :

| Paramètre | Valeur |
| --- | --- |
| Name | `mon-projet-api-audience` |
| Protocol | `OpenID Connect` |

Dans ce scope, ouvrez **Mappers → Configure a new mapper → Audience**, puis configurez :

| Paramètre | Valeur |
| --- | --- |
| Name | `mon-projet-api-audience` |
| Included Client Audience | `mon-projet-api` |
| Add to access token | `ON` |
| Add to ID token | `OFF` |

Ce mapper ajoute la valeur attendue au claim `aud` du token d'accès.

## 4. Créer le client Scalar

Via **Clients → Create client**, créez :

### General settings

| Paramètre | Valeur |
| --- | --- |
| Client type | `OpenID Connect` |
| Client ID | `mon-projet-scalar` |

### Capability config

| Paramètre | Valeur |
| --- | --- |
| Client authentication | `OFF` |
| Standard flow | `ON` |
| Direct access grants | `OFF` |
| Implicit flow | `OFF` |
| Service accounts roles | `OFF` |

Scalar est un client public exécuté dans le navigateur : il ne doit jamais contenir de `client_secret`. Il utilise l'*Authorization Code Flow* avec PKCE.

## 5. Activer PKCE

Dans les paramètres avancés du client `mon-projet-scalar`, sélectionnez :

```text
PKCE method: S256
```

N'utilisez pas `plain`.

## 6. Configurer les URLs Scalar

Si l'API locale est accessible sur `http://localhost:5117` et Scalar sur `http://localhost:5117/scalar/v1`, configurez le client `mon-projet-scalar` ainsi :

| Paramètre | Valeur |
| --- | --- |
| Root URL | `http://localhost:5117` |
| Home URL | `http://localhost:5117/scalar/v1` |
| Valid redirect URIs | `http://localhost:5117/scalar/v1` |
| Valid post logout redirect URIs | `http://localhost:5117/scalar/v1` |
| Web origins | `http://localhost:5117` |

Évitez `*` dans les *redirect URIs* et les *Web origins*.

## 7. Attacher l'audience API à Scalar

Sur le client `mon-projet-scalar`, ouvrez **Client scopes** et ajoutez `mon-projet-api-audience` comme **Default Client Scope**.

```text
Scalar demande un token
        ↓
Client scope mon-projet-api-audience
        ↓
Audience mapper
        ↓
aud = mon-projet-api
        ↓
.NET accepte le token
```

## 8. Créer un utilisateur de test

Ouvrez **Users → Add user**, par exemple :

| Paramètre | Valeur |
| --- | --- |
| Username | `test` |
| Email | `test@example.com` |

Dans **Credentials → Set password**, définissez un mot de passe et mettez **Temporary** sur `OFF`. L'utilisateur ne sera alors pas forcé de modifier son mot de passe lors de sa première connexion.

## 9. Conserver les rôles métier dans l'application

Ne créez pas les rôles métier `Admin` et `Creator` comme *Realm Roles* Keycloak. Ils sont gérés dans PostgreSQL, par `users.users` et `users.user_roles`.

```text
Keycloak
└── identité (sub)

Template.Modules.Users
├── profil utilisateur
└── rôles applicatifs

Template.Modules.Blog
└── autorisation sur les posts
```

Keycloak répond à « qui est l'utilisateur ? » via `sub`. L'application répond à « que peut faire l'utilisateur ? » via les rôles applicatifs et la propriété des posts.

## 10. Configurer .NET

Configurez l'application :

```json
{
  "Authentication": {
    "Authority": "https://<ton-keycloak>/realms/mon-projet",
    "Audience": "mon-projet-api"
  }
}
```

L'authentification du template configure notamment :

```csharp
services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = authenticationOptions.Authority;
        options.Audience = authenticationOptions.Audience;
        options.RequireHttpsMetadata = true;
        options.MapInboundClaims = false;
    });
```

`MapInboundClaims = false` est nécessaire car `CurrentUser` lit directement le claim `sub` avec `User.FindFirstValue("sub")`.

## 11. Configurer Scalar

La configuration Scalar doit référencer le client public et demander les scopes usuels :

```text
Client ID : mon-projet-scalar
PKCE      : S256
Scopes    : openid profile email
```

Dans `Template.Api/Program.cs`, adaptez également la définition du schéma OpenAPI OAuth2 et le flux Scalar. Les URLs d'autorisation et de token doivent cibler le nouveau realm, et le `ClientId` doit correspondre au client Scalar créé dans Keycloak :

```csharp
document.Components.SecuritySchemes["OAuth2"] =
    new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Description = "Keycloak OAuth2 Authorization Code with PKCE",
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(
                    "https://<ton-keycloak>/realms/mon-projet/protocol/openid-connect/auth"),
                TokenUrl = new Uri(
                    "https://<ton-keycloak>/realms/mon-projet/protocol/openid-connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    ["openid"] = "OpenID",
                    ["profile"] = "Profile",
                    ["email"] = "Email"
                }
            }
        }
    };
```

Dans `app.MapScalarApiReference(...)`, vérifiez aussi :

```csharp
options
    .AddPreferredSecuritySchemes("OAuth2")
    .AddAuthorizationCodeFlow("OAuth2", flow =>
    {
        flow.ClientId = "mon-projet-scalar";
        flow.Pkce = Pkce.Sha256;
        flow.SelectedScopes = ["openid", "profile", "email"];
    });
```

Les valeurs actuellement en dur pour `template` et `template-scalar` dans `Program.cs` doivent être remplacées à chaque nouveau projet.

```text
Scalar
  │ Authorization Code + PKCE
  ▼
Keycloak / realm mon-projet
  │ JWT (aud = mon-projet-api, sub = identité utilisateur)
  ▼
ASP.NET Core
  ├── vérifie l'issuer
  ├── vérifie la signature
  ├── vérifie l'audience
  ▼
ICurrentUser.IdentityId
  ▼
Users.IdentityId
```

## 12. Créer le client machine de création d'utilisateurs

L'inscription publique appelle l'API d'administration Keycloak depuis le backend. Créez donc un troisième client, distinct des clients API et Scalar :

```text
joseph-platform-createuser
```

Dans **Clients → Create client**, configurez :

| Paramètre | Valeur |
| --- | --- |
| Client type | `OpenID Connect` |
| Client ID | `joseph-platform-createuser` |
| Client authentication | `ON` |
| Service accounts roles | `ON` |
| Standard flow | `OFF` |
| Direct access grants | `OFF` |
| Implicit flow | `OFF` |

Dans **Clients → joseph-platform-createuser → Service account roles**, attribuez le rôle :

```text
realm-management → manage-users
```

N'attribuez pas `realm-admin` : il est inutilement permissif. `manage-users` suffit pour créer les utilisateurs du realm.

Le secret de ce client doit rester exclusivement côté serveur, par exemple dans les variables d'environnement :

```text
Keycloak__CreateUserClient__ClientSecret=<secret>
```

La configuration associée dans l'API est :

```json
{
  "Keycloak": {
    "CreateUserClient": {
      "Authority": "https://<ton-keycloak>",
      "Realm": "mon-projet",
      "ClientId": "joseph-platform-createuser",
      "ClientSecret": "<secret>"
    }
  }
}
```

L'endpoint public `POST /auth/register` obtient un token `client_credentials`, appelle `POST /admin/realms/{realm}/users`, récupère l'identifiant depuis le header `Location`, puis crée le profil dans `users.users` avec cet identifiant comme `IdentityId`.

```text
POST /auth/register
        ↓
KeycloakIdentityProvider
        ↓ client_credentials
Keycloak Admin API
        ↓
IdentityId Keycloak
        ↓
users.users
```

## 13. Bootstrap du premier administrateur

Après la connexion du nouvel utilisateur, récupérez son `sub` Keycloak puis configurez :

```json
{
  "BootstrapAdmin": {
    "Enabled": true,
    "IdentityId": "<sub-keycloak>"
  }
}
```

Le bootstrap applicatif attribue alors les rôles `Admin` et `Creator` dans PostgreSQL. Aucun rôle Keycloak supplémentaire n'est nécessaire.

## Checklist finale

### Realm

- [ ] Realm créé.

### API

- [ ] Client `mon-projet-api` créé.
- [ ] Client authentication activée.
- [ ] Standard Flow désactivé.

### Audience

- [ ] Client scope `mon-projet-api-audience` créé.
- [ ] Audience mapper créé.
- [ ] Included Client Audience = `mon-projet-api`.
- [ ] Add to access token activé.

### Scalar

- [ ] Client `mon-projet-scalar` créé.
- [ ] Client authentication désactivée.
- [ ] Standard Flow activé.
- [ ] Direct Access Grants désactivé.
- [ ] Implicit Flow désactivé.
- [ ] PKCE = `S256`.
- [ ] Redirect URI configurée.
- [ ] Web Origin configurée.
- [ ] `mon-projet-api-audience` ajouté en Default Client Scope.

### Utilisateur

- [ ] Utilisateur de test créé.
- [ ] Mot de passe défini.
- [ ] Temporary désactivé.

### Création d'utilisateurs

- [ ] Client `joseph-platform-createuser` créé.
- [ ] Client authentication et Service accounts roles activés.
- [ ] Standard Flow, Direct Access Grants et Implicit Flow désactivés.
- [ ] Rôle `realm-management → manage-users` attribué au service account.
- [ ] Secret du client renseigné uniquement côté serveur.
- [ ] `POST /auth/register` crée l'identité Keycloak puis le profil applicatif.

### .NET

- [ ] Authority cible `/realms/mon-projet`.
- [ ] Audience = `mon-projet-api`.
- [ ] `MapInboundClaims = false`.
- [ ] Les URLs OAuth2 dans `Program.cs` ciblent le realm `mon-projet`.
- [ ] Le `ClientId` du flux Scalar dans `Program.cs` est `mon-projet-scalar`.

### Test final

- [ ] Connexion Scalar fonctionnelle.
- [ ] `access_token` reçu.
- [ ] Token contenant `sub`.
- [ ] Token contenant `aud = mon-projet-api`.
- [ ] Endpoint `RequireAuthorization` renvoyant `200` ou `204` avec le token.
- [ ] Même endpoint renvoyant `401` sans token.
