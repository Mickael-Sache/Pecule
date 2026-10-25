# Pécule

[![CI](https://github.com/Mickael-Sache/pecule/actions/workflows/ci.yml/badge.svg)](https://github.com/Mickael-Sache/pecule/actions/workflows/ci.yml)

Application de gestion de budget personnel : import de relevés bancaires, analyse des dépenses et alertes budgétaires. Projet de portfolio en .NET 10 et Angular 22, conçu pour être conforme au RGAA 4.1 / WCAG 2.2 AA.

> Projet en cours de construction : les fondations (architecture, conteneurisation, CI) sont en place, les fonctionnalités arrivent étape par étape.

## Stack

| Domaine | Technologies |
| --- | --- |
| Backend | .NET 10, ASP.NET Core Minimal APIs, OpenAPI + Scalar |
| Architecture | Monolithe modulaire (Accounts, Imports, Insights, Notifications), tests d'architecture |
| Données | PostgreSQL 17, EF Core 10, un schéma par module |
| Front | Angular 22 (signals, zoneless, `httpResource`), SCSS |
| Tests | xUnit, Vitest, angular-eslint (règles d'accessibilité) |
| Conteneurs | Docker multi-stage, docker compose |
| CI | GitHub Actions |

## Démarrage rapide

Prérequis : [Docker Desktop](https://www.docker.com/products/docker-desktop/).

```bash
cp .env.example .env        # sous Windows : Copy-Item .env.example .env
# renseigner POSTGRES_PASSWORD dans .env
docker compose up --build
```

Puis ouvrir :

| Adresse | Contenu |
| --- | --- |
| http://localhost:8080 | l'application |
| http://localhost:8080/scalar | la documentation de l'API |
| http://localhost:8080/health | l'état de santé (base de données comprise) |

## Développement

Prérequis : SDK .NET 10, Node.js 24, Angular CLI 22, Docker.

```bash
# Base de données
docker compose up -d db

# Migrations (première fois)
dotnet tool restore
dotnet ef database update --project src/Modules/Accounts/Pecule.Accounts --startup-project src/Pecule.Host --context AccountsDbContext

# API
dotnet run --project src/Pecule.Host

# Front, dans un second terminal (proxy vers l'API configuré)
cd front
npm install
ng serve
```

Le front est disponible sur http://localhost:4200.

La chaîne de connexion de développement se configure avec les secrets utilisateur :

```bash
dotnet user-secrets set "ConnectionStrings:Pecule" "Host=localhost;Database=pecule;Username=pecule;Password=<mot de passe du .env>" --project src/Pecule.Host
```

## Tests

```bash
dotnet test           # backend, dont les tests d'architecture
cd front
ng lint               # dont les règles d'accessibilité des templates
ng test               # tests unitaires Vitest
```

## Structure

```
src/
  Pecule.Host/          point d'entrée : API et service de l'application Angular
  Modules/              Accounts, Imports, Insights, Notifications (+ leurs Contracts)
  Shared/               noyau partagé
front/                  application Angular 22
tests/                  tests d'architecture
```

Chaque module ne connaît que les contrats publics des autres ; un test d'architecture fait échouer la CI en cas de dépendance interdite.

## Accessibilité

Objectif : conformité RGAA 4.1 / WCAG 2.2 niveau AA. Déjà en place : lien d'évitement, landmarks, titres de page par route, focus replacé sur le titre principal après chaque navigation, règles d'accessibilité d'angular-eslint dans la CI. L'audit complet et la déclaration d'accessibilité sont prévus en fin de MVP.
