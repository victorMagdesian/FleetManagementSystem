# FleetManager Frontend

Sistema de gerenciamento de frotas de veículos - Interface Web

## Tecnologias

- **Angular 18** - Framework principal
- **PrimeNG 17** - Biblioteca de componentes UI
- **PrimeIcons** - Ícones
- **PrimeFlex** - Utilitários CSS
- **TypeScript 5** - Linguagem
- **SCSS** - Estilização

## Pré-requisitos

- Node.js 18+ 
- npm 9+

## Instalação

```bash
# Instalar dependências
npm install
```

## Desenvolvimento

```bash
# Iniciar servidor de desenvolvimento
npm start

# Ou
ng serve
```

A aplicação estará disponível em `http://localhost:4200/`

## Build

```bash
# Build de desenvolvimento
npm run build

# Build de produção
npm run build -- --configuration production
```

## Estrutura do Projeto

```
src/
├── app/
│   ├── core/                    # Serviços globais, interceptors, guards
│   ├── shared/                  # Componentes compartilhados (Atomic Design)
│   │   ├── components/
│   │   │   ├── atoms/          # Componentes básicos
│   │   │   ├── molecules/      # Combinações simples
│   │   │   └── organisms/      # Componentes complexos
│   │   ├── pipes/              # Pipes customizados
│   │   └── directives/         # Diretivas customizadas
│   ├── features/               # Módulos de funcionalidades
│   │   ├── dashboard/
│   │   ├── vehicles/
│   │   ├── drivers/
│   │   ├── trips/
│   │   └── maintenance/
│   └── layout/                 # Templates de layout
├── environments/               # Configurações de ambiente
└── styles/                     # Estilos globais e variáveis
```

## Convenções de Código

- Usar standalone components
- Seguir Atomic Design para organização de componentes
- Usar SCSS para estilização
- Implementar lazy loading para módulos de features
- Usar RxJS para programação reativa
- Seguir guia de estilo oficial do Angular

## Configuração da API

A URL da API é configurada nos arquivos de environment:

- **Desenvolvimento**: `src/environments/environment.ts`
- **Produção**: `src/environments/environment.prod.ts`

## Scripts Disponíveis

- `npm start` - Inicia servidor de desenvolvimento
- `npm run build` - Build de produção
- `npm test` - Executa testes unitários
- `npm run lint` - Executa linter

## Locale

A aplicação está configurada para usar o locale pt-BR (português brasileiro) para formatação de datas, números e moeda.
