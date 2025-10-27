# 🏛️ Sistema de Gestão de Biblioteca - Clean Architecture & DDD

## 📋 Descrição
Sistema completo de gestão de biblioteca desenvolvido com Clean Architecture, Domain-Driven Design (DDD) e Entity Framework Core.

## 🏗️ Arquitetura
- **Domínio**: Entidades, agregados e contratos de repositório
- **Aplicação**: DTOs, serviços e casos de uso
- **Infraestrutura**: Entity Framework, repositórios concretos, SQLite
- **Apresentação**: API REST com controllers

## 🚀 Como Executar

### Pré-requisitos
- .NET 8.0 SDK
- Git

### Passos
```bash
# Clonar repositório
git clone https://github.com/seu-usuario/library-management.git
cd library-management

# Executar aplicação
cd src/LibraryManagement.API
dotnet run

# Acessar Swagger
# http://localhost:5291/swagger