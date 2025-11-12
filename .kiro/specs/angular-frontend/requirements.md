# Documento de Requisitos - FleetManager Frontend

## Introdução

O FleetManager Frontend é uma aplicação web moderna construída com Angular 18, PrimeNG e seguindo os princípios de Atomic Design. A aplicação fornece uma interface completa para gerenciamento de frotas, permitindo que empresas de transporte e turismo gerenciem veículos, condutores, manutenções e viagens de forma eficiente e intuitiva.

## Glossário

- **Sistema_Frontend**: A aplicação web Angular que consome a API REST do FleetManager
- **PrimeNG**: Biblioteca de componentes UI para Angular
- **Atomic_Design**: Metodologia de design que organiza componentes em átomos, moléculas, organismos, templates e páginas
- **Usuário**: Funcionário da empresa de transporte que utiliza o sistema
- **Veículo**: Automóvel da frota que pode estar Disponível, Em Uso ou Em Manutenção
- **Condutor**: Motorista habilitado que pode operar os veículos
- **Viagem**: Registro de uso de um veículo por um condutor em uma rota específica
- **Manutenção**: Registro de serviço realizado em um veículo
- **API_Backend**: Serviço REST ASP.NET Core que fornece os dados

## Requisitos

### Requisito 1

**User Story:** Como usuário do sistema, quero visualizar um dashboard com informações resumidas da frota, para que eu possa ter uma visão geral rápida do status operacional

#### Acceptance Criteria

1. WHEN o Usuário acessa a página inicial, THE Sistema_Frontend SHALL exibir cards com totais de veículos disponíveis, em uso e em manutenção
2. WHEN o Usuário acessa a página inicial, THE Sistema_Frontend SHALL exibir lista de veículos com manutenção próxima nos próximos 7 dias
3. WHEN o Usuário acessa a página inicial, THE Sistema_Frontend SHALL exibir lista de viagens ativas no momento
4. WHEN o Usuário acessa a página inicial, THE Sistema_Frontend SHALL carregar os dados do API_Backend em até 3 segundos

### Requisito 2

**User Story:** Como usuário do sistema, quero gerenciar o cadastro de veículos, para que eu possa manter a frota atualizada

#### Acceptance Criteria

1. WHEN o Usuário acessa a página de veículos, THE Sistema_Frontend SHALL exibir tabela com todos os veículos cadastrados incluindo placa, modelo, ano, quilometragem e status
2. WHEN o Usuário clica no botão "Novo Veículo", THE Sistema_Frontend SHALL exibir formulário modal com campos para placa, modelo, ano e quilometragem
3. WHEN o Usuário preenche o formulário e clica em "Salvar", THE Sistema_Frontend SHALL enviar requisição POST para API_Backend e atualizar a lista
4. WHEN o Usuário clica no botão "Editar" de um veículo, THE Sistema_Frontend SHALL exibir formulário modal preenchido com dados atuais
5. WHEN o Usuário clica no botão "Excluir" de um veículo, THE Sistema_Frontend SHALL exibir diálogo de confirmação antes de enviar requisição DELETE
6. WHEN o Usuário digita no campo de busca, THE Sistema_Frontend SHALL filtrar a tabela em tempo real por placa ou modelo
7. IF a API_Backend retorna erro 409, THEN THE Sistema_Frontend SHALL exibir mensagem "Veículo com esta placa já existe"

### Requisito 3

**User Story:** Como usuário do sistema, quero gerenciar o cadastro de condutores, para que eu possa controlar quem está autorizado a dirigir os veículos

#### Acceptance Criteria

1. WHEN o Usuário acessa a página de condutores, THE Sistema_Frontend SHALL exibir tabela com todos os condutores incluindo nome, CNH, telefone e status ativo/inativo
2. WHEN o Usuário clica no botão "Novo Condutor", THE Sistema_Frontend SHALL exibir formulário modal com campos para nome, CNH e telefone
3. WHEN o Usuário preenche o formulário e clica em "Salvar", THE Sistema_Frontend SHALL validar formato da CNH (11 dígitos) antes de enviar
4. WHEN o Usuário clica no botão "Editar" de um condutor, THE Sistema_Frontend SHALL exibir formulário modal preenchido com dados atuais
5. WHEN o Usuário clica no botão "Ativar/Desativar", THE Sistema_Frontend SHALL enviar requisição POST para endpoint apropriado e atualizar status
6. WHEN o Usuário clica no botão "Excluir" de um condutor, THE Sistema_Frontend SHALL exibir diálogo de confirmação antes de enviar requisição DELETE
7. IF a API_Backend retorna erro 409, THEN THE Sistema_Frontend SHALL exibir mensagem "Condutor com esta CNH já existe"

### Requisito 4

**User Story:** Como usuário do sistema, quero iniciar e finalizar viagens, para que eu possa registrar o uso dos veículos e atualizar a quilometragem

#### Acceptance Criteria

1. WHEN o Usuário acessa a página de viagens, THE Sistema_Frontend SHALL exibir tabela com todas as viagens incluindo veículo, condutor, rota, datas e distância
2. WHEN o Usuário clica no botão "Iniciar Viagem", THE Sistema_Frontend SHALL exibir formulário modal com dropdowns de veículos disponíveis e condutores ativos
3. WHEN o Usuário seleciona veículo e condutor e preenche a rota, THE Sistema_Frontend SHALL enviar requisição POST para /api/trips/start
4. WHEN o Usuário clica no botão "Finalizar" de uma viagem ativa, THE Sistema_Frontend SHALL exibir formulário modal solicitando a distância percorrida
5. WHEN o Usuário informa a distância e confirma, THE Sistema_Frontend SHALL enviar requisição POST para /api/trips/end/{id} com a distância
6. WHEN o Usuário acessa a aba "Viagens Ativas", THE Sistema_Frontend SHALL exibir apenas viagens sem data de término
7. IF a API_Backend retorna erro 400, THEN THE Sistema_Frontend SHALL exibir mensagem de erro específica retornada pela API

### Requisito 5

**User Story:** Como usuário do sistema, quero registrar manutenções realizadas nos veículos, para que eu possa manter histórico e controlar próximas manutenções

#### Acceptance Criteria

1. WHEN o Usuário acessa a página de manutenções, THE Sistema_Frontend SHALL exibir tabela com todas as manutenções incluindo veículo, data, descrição e custo
2. WHEN o Usuário clica no botão "Nova Manutenção", THE Sistema_Frontend SHALL exibir formulário modal com dropdown de veículos, data, descrição e custo
3. WHEN o Usuário preenche o formulário e clica em "Salvar", THE Sistema_Frontend SHALL enviar requisição POST para /api/maintenance
4. WHEN o Usuário clica em um veículo na tabela, THE Sistema_Frontend SHALL exibir modal com histórico completo de manutenções daquele veículo
5. WHEN o Usuário acessa a aba "Manutenções Próximas", THE Sistema_Frontend SHALL exibir veículos com manutenção prevista nos próximos 7 dias
6. WHEN o Usuário visualiza uma manutenção próxima, THE Sistema_Frontend SHALL destacar visualmente com badge de alerta

### Requisito 6

**User Story:** Como usuário do sistema, quero navegar facilmente entre as diferentes seções, para que eu possa acessar rapidamente as funcionalidades necessárias

#### Acceptance Criteria

1. THE Sistema_Frontend SHALL exibir menu lateral com links para Dashboard, Veículos, Condutores, Viagens e Manutenções
2. WHEN o Usuário clica em um item do menu, THE Sistema_Frontend SHALL navegar para a página correspondente e destacar o item ativo
3. THE Sistema_Frontend SHALL exibir barra superior com título da página atual e informações do usuário
4. WHEN o Usuário acessa o sistema em dispositivo móvel, THE Sistema_Frontend SHALL exibir menu hambúrguer colapsável
5. THE Sistema_Frontend SHALL manter estado do menu (expandido/colapsado) durante a navegação

### Requisito 7

**User Story:** Como usuário do sistema, quero receber feedback visual das ações realizadas, para que eu saiba se as operações foram bem-sucedidas ou falharam

#### Acceptance Criteria

1. WHEN uma operação é concluída com sucesso, THE Sistema_Frontend SHALL exibir toast de sucesso com mensagem descritiva por 3 segundos
2. WHEN uma operação falha, THE Sistema_Frontend SHALL exibir toast de erro com mensagem descritiva por 5 segundos
3. WHILE uma requisição está em andamento, THE Sistema_Frontend SHALL exibir indicador de carregamento (spinner) no componente afetado
4. WHEN o Usuário tenta submeter formulário com campos inválidos, THE Sistema_Frontend SHALL exibir mensagens de validação abaixo de cada campo
5. WHEN o Usuário passa o mouse sobre botões de ação, THE Sistema_Frontend SHALL exibir tooltip explicativo

### Requisito 8

**User Story:** Como usuário do sistema, quero visualizar badges de status nos veículos, para que eu possa identificar rapidamente a situação de cada um

#### Acceptance Criteria

1. WHEN o Sistema_Frontend exibe um veículo com status "Available", THE Sistema_Frontend SHALL exibir badge verde com texto "Disponível"
2. WHEN o Sistema_Frontend exibe um veículo com status "InUse", THE Sistema_Frontend SHALL exibir badge azul com texto "Em Uso"
3. WHEN o Sistema_Frontend exibe um veículo com status "InMaintenance", THE Sistema_Frontend SHALL exibir badge laranja com texto "Em Manutenção"
4. WHEN o Sistema_Frontend exibe um condutor ativo, THE Sistema_Frontend SHALL exibir badge verde com texto "Ativo"
5. WHEN o Sistema_Frontend exibe um condutor inativo, THE Sistema_Frontend SHALL exibir badge cinza com texto "Inativo"

### Requisito 9

**User Story:** Como usuário do sistema, quero que a aplicação seja responsiva, para que eu possa utilizá-la em diferentes dispositivos

#### Acceptance Criteria

1. WHEN o Usuário acessa o sistema em desktop (>1024px), THE Sistema_Frontend SHALL exibir layout com menu lateral expandido e tabelas completas
2. WHEN o Usuário acessa o sistema em tablet (768px-1024px), THE Sistema_Frontend SHALL ajustar tabelas com scroll horizontal quando necessário
3. WHEN o Usuário acessa o sistema em mobile (<768px), THE Sistema_Frontend SHALL exibir menu colapsado e cards ao invés de tabelas
4. THE Sistema_Frontend SHALL manter funcionalidades completas em todos os tamanhos de tela
5. WHEN o Usuário redimensiona a janela, THE Sistema_Frontend SHALL adaptar o layout dinamicamente sem necessidade de reload

### Requisito 10

**User Story:** Como usuário do sistema, quero que os dados sejam validados antes do envio, para que eu evite erros e retrabalho

#### Acceptance Criteria

1. WHEN o Usuário tenta salvar veículo sem preencher placa, THE Sistema_Frontend SHALL exibir mensagem "Placa é obrigatória"
2. WHEN o Usuário tenta salvar veículo com ano inválido, THE Sistema_Frontend SHALL exibir mensagem "Ano deve estar entre 1900 e ano atual + 1"
3. WHEN o Usuário tenta salvar condutor sem CNH válida, THE Sistema_Frontend SHALL exibir mensagem "CNH deve conter 11 dígitos"
4. WHEN o Usuário tenta salvar manutenção com custo negativo, THE Sistema_Frontend SHALL exibir mensagem "Custo deve ser maior que zero"
5. WHEN o Usuário tenta finalizar viagem com distância zero ou negativa, THE Sistema_Frontend SHALL exibir mensagem "Distância deve ser maior que zero"
6. THE Sistema_Frontend SHALL desabilitar botão de submit enquanto formulário contiver erros de validação

### Requisito 11

**User Story:** Como usuário do sistema, quero que a aplicação tenha boa performance, para que eu possa trabalhar de forma ágil

#### Acceptance Criteria

1. WHEN o Usuário navega entre páginas, THE Sistema_Frontend SHALL carregar nova página em menos de 1 segundo
2. WHEN o Usuário filtra tabelas, THE Sistema_Frontend SHALL aplicar filtro em menos de 500 milissegundos
3. THE Sistema_Frontend SHALL implementar lazy loading para módulos de funcionalidades
4. THE Sistema_Frontend SHALL cachear dados de veículos disponíveis e condutores ativos por 5 minutos
5. WHEN o Usuário realiza múltiplas ações rapidamente, THE Sistema_Frontend SHALL prevenir requisições duplicadas

### Requisito 12

**User Story:** Como desenvolvedor, quero que a aplicação siga Atomic Design, para que o código seja organizado, reutilizável e mantível

#### Acceptance Criteria

1. THE Sistema_Frontend SHALL organizar componentes em estrutura atoms/, molecules/, organisms/, templates/ e pages/
2. THE Sistema_Frontend SHALL implementar átomos como botões, inputs, badges e ícones reutilizáveis
3. THE Sistema_Frontend SHALL implementar moléculas como campos de formulário com label e validação
4. THE Sistema_Frontend SHALL implementar organismos como tabelas, formulários completos e cards de dashboard
5. THE Sistema_Frontend SHALL implementar templates como layouts de página com menu e header
6. THE Sistema_Frontend SHALL implementar pages como componentes de rota que compõem templates com dados

### Requisito 13

**User Story:** Como usuário do sistema, quero visualizar informações formatadas corretamente, para que eu possa ler e entender os dados facilmente

#### Acceptance Criteria

1. WHEN o Sistema_Frontend exibe datas, THE Sistema_Frontend SHALL formatar no padrão brasileiro "dd/MM/yyyy HH:mm"
2. WHEN o Sistema_Frontend exibe valores monetários, THE Sistema_Frontend SHALL formatar com "R$" e duas casas decimais
3. WHEN o Sistema_Frontend exibe quilometragem, THE Sistema_Frontend SHALL formatar com separador de milhares e sufixo "km"
4. WHEN o Sistema_Frontend exibe telefone, THE Sistema_Frontend SHALL formatar no padrão brasileiro "+55 (XX) XXXXX-XXXX"
5. WHEN o Sistema_Frontend exibe placas de veículos, THE Sistema_Frontend SHALL exibir em letras maiúsculas

### Requisito 14

**User Story:** Como usuário do sistema, quero que erros de conexão sejam tratados adequadamente, para que eu saiba quando há problemas de comunicação com o servidor

#### Acceptance Criteria

1. IF a API_Backend não responde em 30 segundos, THEN THE Sistema_Frontend SHALL exibir mensagem "Tempo de conexão esgotado. Tente novamente"
2. IF a API_Backend retorna erro 500, THEN THE Sistema_Frontend SHALL exibir mensagem "Erro interno do servidor. Contate o suporte"
3. IF a API_Backend retorna erro 404, THEN THE Sistema_Frontend SHALL exibir mensagem "Recurso não encontrado"
4. IF não há conexão com internet, THEN THE Sistema_Frontend SHALL exibir mensagem "Sem conexão com internet. Verifique sua rede"
5. WHEN ocorre erro de conexão, THE Sistema_Frontend SHALL oferecer botão "Tentar Novamente" para repetir a operação
