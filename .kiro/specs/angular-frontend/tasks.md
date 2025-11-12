# Plano de Implementação - FleetManager Frontend

- [x] 1. Configurar projeto Angular e dependências





  - Criar novo projeto Angular 18 com standalone components
  - Instalar e configurar PrimeNG, PrimeIcons e PrimeFlex
  - Configurar SCSS e variáveis de estilo
  - Configurar locale pt-BR e timezone
  - Configurar environment files com URL da API
  - _Requirements: 12.1, 13.4_

- [ ] 2. Implementar estrutura core do projeto
- [ ] 2.1 Criar modelos de dados TypeScript
  - Criar interfaces para Vehicle, Driver, Trip, Maintenance
  - Criar enums para VehicleStatus
  - Criar interfaces para requests (Create/Update)
  - Criar interface DashboardStats
  - _Requirements: 1.1, 2.1, 3.1, 4.1, 5.1_

- [ ] 2.2 Implementar serviços de API
  - Criar ApiService base com métodos HTTP genéricos
  - Criar VehicleService com todos os endpoints
  - Criar DriverService com todos os endpoints
  - Criar TripService com todos os endpoints
  - Criar MaintenanceService com todos os endpoints
  - Criar ToastService para notificações
  - _Requirements: 7.1, 7.2, 14.1, 14.2, 14.3, 14.4_

- [ ] 2.3 Implementar interceptors HTTP
  - Criar ErrorInterceptor para tratamento global de erros
  - Criar LoadingInterceptor para controle de loading state
  - Configurar interceptors no app.config
  - _Requirements: 7.2, 7.3, 14.1, 14.2, 14.3, 14.4, 14.5_

- [ ] 3. Implementar componentes Atomic Design - Átomos
- [ ] 3.1 Criar componente Button
  - Implementar wrapper do p-button com inputs customizados
  - Adicionar suporte a loading e disabled states
  - Adicionar tooltips
  - _Requirements: 7.5, 12.2_

- [ ] 3.2 Criar componente Badge
  - Implementar wrapper do p-tag
  - Adicionar suporte a diferentes severities
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 12.2_

- [ ] 3.3 Criar componente Icon
  - Implementar wrapper para PrimeIcons
  - Adicionar suporte a tamanhos e cores
  - _Requirements: 12.2_

- [ ] 4. Implementar componentes Atomic Design - Moléculas
- [ ] 4.1 Criar componente FormField
  - Implementar container com label, input slot e mensagem de erro
  - Adicionar indicador de campo obrigatório
  - Estilizar estados de validação
  - _Requirements: 7.4, 10.6, 12.3_

- [ ] 4.2 Criar componente SearchBox
  - Implementar input de busca com ícone
  - Adicionar debounce de 300ms
  - Emitir evento de busca
  - _Requirements: 2.6, 11.2, 12.3_

- [ ] 4.3 Criar componente StatusBadge
  - Implementar lógica de mapeamento de status para labels pt-BR
  - Implementar lógica de mapeamento de status para cores
  - Usar componente Badge internamente
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 12.3_

- [ ] 5. Implementar componentes Atomic Design - Organismos
- [ ] 5.1 Criar componente DataTable
  - Implementar wrapper do p-table com header customizado
  - Integrar SearchBox no header
  - Integrar Button para adicionar no header
  - Adicionar paginação e loading state
  - Implementar filtro global
  - _Requirements: 2.1, 2.6, 3.1, 4.1, 5.1, 7.3, 11.2, 12.4_

- [ ] 5.2 Criar componente FormDialog
  - Implementar wrapper do p-dialog
  - Adicionar footer com botões Cancelar e Salvar
  - Adicionar loading state no botão Salvar
  - Controlar habilitação do botão baseado em validação
  - _Requirements: 2.2, 2.4, 3.2, 3.4, 4.2, 4.4, 5.2, 7.3, 10.6, 12.4_

- [ ] 5.3 Criar componente StatsCard
  - Implementar card com ícone, valor e label
  - Adicionar suporte a diferentes cores
  - Estilizar com SCSS responsivo
  - _Requirements: 1.1, 9.1, 12.4_

- [ ] 6. Implementar pipes customizados
- [ ] 6.1 Criar DateFormatPipe
  - Implementar formatação de datas no padrão brasileiro
  - Suportar diferentes formatos
  - _Requirements: 13.1_

- [ ] 6.2 Criar CurrencyFormatPipe
  - Implementar formatação monetária com R$
  - Usar Intl.NumberFormat com locale pt-BR
  - _Requirements: 13.2_

- [ ] 6.3 Criar PhoneFormatPipe
  - Implementar formatação de telefone brasileiro
  - Suportar formato +55 (XX) XXXXX-XXXX
  - _Requirements: 13.4_

- [ ] 6.4 Criar VehicleStatusPipe
  - Implementar mapeamento de status para português
  - _Requirements: 8.1, 8.2, 8.3, 13.5_


- [ ] 7. Implementar layout principal
- [ ] 7.1 Criar componente MainLayout
  - Implementar estrutura com sidebar e área de conteúdo
  - Adicionar router-outlet para páginas
  - Adicionar p-toast para notificações
  - Adicionar p-confirmDialog para confirmações
  - _Requirements: 6.1, 6.2, 6.3, 7.1, 7.2, 12.5_

- [ ] 7.2 Criar componente Header
  - Implementar barra superior com título da página
  - Adicionar botão de toggle do menu para mobile
  - Estilizar responsivamente
  - _Requirements: 6.3, 9.3, 12.5_

- [ ] 7.3 Criar componente Sidebar
  - Implementar menu lateral com itens de navegação
  - Adicionar ícones e labels para cada item
  - Implementar estado expandido/colapsado
  - Destacar item ativo
  - Tornar responsivo com menu hambúrguer em mobile
  - _Requirements: 6.1, 6.2, 6.4, 6.5, 9.1, 9.3, 12.5_

- [ ] 8. Configurar sistema de rotas
- [ ] 8.1 Criar arquivo de rotas principal
  - Configurar rota raiz com MainLayout
  - Configurar rotas filhas para cada módulo
  - Implementar lazy loading para todos os módulos
  - Configurar redirect padrão para dashboard
  - Configurar rota wildcard para 404
  - _Requirements: 6.2, 11.1, 11.3_

- [ ] 9. Implementar módulo Dashboard
- [ ] 9.1 Criar DashboardPage component
  - Implementar layout com grid de stats cards
  - Implementar seções para manutenções próximas e viagens ativas
  - Carregar dados usando forkJoin
  - Calcular estatísticas de veículos
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 11.1_

- [ ] 9.2 Criar componente FleetSummary
  - Exibir 4 StatsCards com totais de veículos
  - Aplicar cores apropriadas para cada card
  - _Requirements: 1.1_

- [ ] 9.3 Criar componente UpcomingMaintenance
  - Exibir lista de veículos com manutenção próxima
  - Usar DataTable ou lista customizada
  - Destacar com badges de alerta
  - _Requirements: 1.2, 5.6_

- [ ] 9.4 Criar componente ActiveTrips
  - Exibir lista de viagens ativas
  - Mostrar veículo, condutor e rota
  - _Requirements: 1.3_

- [ ] 10. Implementar módulo Vehicles
- [ ] 10.1 Criar VehiclesPage component
  - Implementar DataTable com todas as colunas
  - Adicionar botões de ação (editar, excluir)
  - Implementar busca por placa e modelo
  - Integrar VehicleForm dialog
  - Implementar confirmação de exclusão
  - _Requirements: 2.1, 2.2, 2.4, 2.5, 2.6, 11.1, 11.2_

- [ ] 10.2 Criar VehicleForm component
  - Implementar formulário com campos placa, modelo, ano e quilometragem
  - Adicionar validações (required, pattern, min, max)
  - Implementar modo criação e edição
  - Desabilitar campo placa em modo edição
  - Exibir mensagens de erro de validação
  - Implementar submit com loading state
  - _Requirements: 2.2, 2.3, 2.4, 2.7, 7.1, 7.2, 7.3, 7.4, 10.1, 10.2, 10.6_

- [ ] 10.3 Criar VehicleDetails component (opcional)
  - Exibir detalhes completos do veículo
  - Mostrar histórico de manutenções
  - _Requirements: 2.1_

- [ ] 11. Implementar módulo Drivers
- [ ] 11.1 Criar DriversPage component
  - Implementar DataTable com todas as colunas
  - Adicionar botões de ação (editar, excluir, ativar/desativar)
  - Implementar busca por nome ou CNH
  - Integrar DriverForm dialog
  - Implementar confirmação de exclusão
  - _Requirements: 3.1, 3.4, 3.5, 3.6, 11.1, 11.2_

- [ ] 11.2 Criar DriverForm component
  - Implementar formulário com campos nome, CNH e telefone
  - Adicionar validações (required, CNH com 11 dígitos)
  - Implementar modo criação e edição
  - Exibir mensagens de erro de validação
  - Implementar submit com loading state
  - _Requirements: 3.2, 3.3, 3.4, 3.7, 7.1, 7.2, 7.3, 7.4, 10.3, 10.6_

- [ ] 12. Implementar módulo Trips
- [ ] 12.1 Criar TripsPage component
  - Implementar DataTable com todas as colunas
  - Adicionar abas para "Todas" e "Ativas"
  - Adicionar botão "Iniciar Viagem"
  - Adicionar botão "Finalizar" para viagens ativas
  - Integrar StartTripForm e EndTripForm dialogs
  - _Requirements: 4.1, 4.2, 4.4, 4.6, 11.1_

- [ ] 12.2 Criar StartTripForm component
  - Implementar formulário com dropdowns de veículos e condutores
  - Carregar apenas veículos disponíveis
  - Carregar apenas condutores ativos
  - Adicionar campo de rota
  - Implementar validações
  - Implementar submit com tratamento de erro 400
  - _Requirements: 4.2, 4.3, 4.7, 7.1, 7.2, 10.6_

- [ ] 12.3 Criar EndTripForm component
  - Implementar formulário com campo de distância
  - Adicionar validação de distância maior que zero
  - Implementar submit com loading state
  - _Requirements: 4.4, 4.5, 7.1, 7.2, 10.5, 10.6_

- [ ] 13. Implementar módulo Maintenance
- [ ] 13.1 Criar MaintenancePage component
  - Implementar DataTable com todas as colunas
  - Adicionar abas para "Todas" e "Próximas"
  - Adicionar botão "Nova Manutenção"
  - Implementar click em veículo para ver histórico
  - Integrar MaintenanceForm dialog
  - _Requirements: 5.1, 5.2, 5.4, 5.5, 11.1_

- [ ] 13.2 Criar MaintenanceForm component
  - Implementar formulário com dropdown de veículos, data, descrição e custo
  - Adicionar validações (required, custo maior que zero)
  - Implementar submit com loading state
  - _Requirements: 5.2, 5.3, 7.1, 7.2, 10.4, 10.6_

- [ ] 13.3 Criar MaintenanceHistory component
  - Implementar modal com histórico de manutenções do veículo
  - Exibir lista ordenada por data
  - Formatar datas e valores monetários
  - _Requirements: 5.4, 13.1, 13.2_

- [ ] 14. Implementar responsividade
- [ ] 14.1 Adicionar media queries e breakpoints
  - Configurar variáveis SCSS para breakpoints
  - Criar mixins para responsive design
  - _Requirements: 9.1, 9.2, 9.3, 9.5_

- [ ] 14.2 Adaptar layout para mobile
  - Ajustar sidebar para menu colapsável
  - Adaptar tabelas com scroll horizontal
  - Converter tabelas em cards em telas pequenas
  - Ajustar espaçamentos e tamanhos de fonte
  - _Requirements: 9.1, 9.2, 9.3, 9.4_

- [ ] 14.3 Testar em diferentes resoluções
  - Testar em desktop (>1024px)
  - Testar em tablet (768px-1024px)
  - Testar em mobile (<768px)
  - _Requirements: 9.1, 9.2, 9.3, 9.4, 9.5_

- [ ] 15. Implementar otimizações de performance
- [ ] 15.1 Configurar lazy loading de módulos
  - Verificar que todos os módulos usam loadChildren
  - Testar carregamento sob demanda
  - _Requirements: 11.1, 11.3_

- [ ] 15.2 Implementar caching de dados
  - Adicionar cache de 5 minutos para veículos disponíveis
  - Adicionar cache de 5 minutos para condutores ativos
  - Usar shareReplay nos observables
  - _Requirements: 11.4_

- [ ] 15.3 Adicionar debounce em campos de busca
  - Implementar debounce de 300ms no SearchBox
  - Testar performance de filtros
  - _Requirements: 11.2_

- [ ] 15.4 Implementar prevenção de requisições duplicadas
  - Adicionar lógica para cancelar requisições pendentes
  - Desabilitar botões durante submissão
  - _Requirements: 11.5_

- [ ] 16. Configurar internacionalização pt-BR
- [ ] 16.1 Configurar locale Angular
  - Registrar locale pt-BR
  - Configurar LOCALE_ID provider
  - _Requirements: 13.1, 13.2, 13.4_

- [ ] 16.2 Configurar traduções PrimeNG
  - Traduzir labels de calendário
  - Traduzir labels de tabela (paginação, filtros)
  - Traduzir labels de dialogs
  - _Requirements: 13.1_

- [ ] 17. Implementar acessibilidade
- [ ] 17.1 Adicionar labels e ARIA attributes
  - Adicionar labels em todos os campos de formulário
  - Adicionar ARIA labels em botões com apenas ícones
  - Adicionar roles apropriados
  - _Requirements: 7.5_

- [ ] 17.2 Garantir navegação por teclado
  - Testar navegação com Tab
  - Garantir focus visível
  - Adicionar atalhos de teclado onde apropriado
  - _Requirements: 6.2_

- [ ] 17.3 Verificar contraste de cores
  - Validar contraste mínimo de 4.5:1
  - Ajustar cores se necessário
  - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5_

- [ ] 18. Criar estilos globais e temas
- [ ] 18.1 Configurar variáveis SCSS
  - Definir cores primárias, secundárias e de status
  - Definir tamanhos de fonte e espaçamentos
  - Definir breakpoints responsivos
  - _Requirements: 9.1, 9.2, 9.3_

- [ ] 18.2 Criar mixins reutilizáveis
  - Criar mixin para responsive
  - Criar mixin para cards
  - Criar mixin para flex layouts
  - _Requirements: 9.1, 9.2, 9.3_

- [ ] 18.3 Configurar tema PrimeNG
  - Escolher e importar tema PrimeNG
  - Customizar cores do tema se necessário
  - Importar PrimeIcons e PrimeFlex
  - _Requirements: 8.1, 8.2, 8.3_

- [ ] 19. Configurar ambiente de produção
- [ ] 19.1 Configurar environment.prod.ts
  - Definir URL da API de produção
  - Configurar flags de produção
  - _Requirements: 2.3, 3.3, 4.3, 4.5, 5.3_

- [ ] 19.2 Otimizar build de produção
  - Configurar build com AOT compilation
  - Habilitar minificação e tree shaking
  - Configurar source maps para produção
  - _Requirements: 11.1, 11.3_

- [ ] 20. Documentação e README
- [ ] 20.1 Criar README.md do frontend
  - Documentar pré-requisitos e instalação
  - Documentar comandos de desenvolvimento
  - Documentar estrutura do projeto
  - Documentar convenções de código
  - _Requirements: 12.1, 12.2, 12.3, 12.4, 12.5, 12.6_

- [ ] 20.2 Documentar componentes principais
  - Adicionar JSDoc nos componentes
  - Documentar inputs e outputs
  - Adicionar exemplos de uso
  - _Requirements: 12.1, 12.2, 12.3, 12.4, 12.5, 12.6_
