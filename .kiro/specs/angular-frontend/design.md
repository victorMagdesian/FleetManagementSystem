# Documento de Design - FleetManager Frontend

## Visão Geral

O FleetManager Frontend é uma Single Page Application (SPA) construída com Angular 18, utilizando PrimeNG como biblioteca de componentes UI e seguindo a metodologia Atomic Design para organização de componentes. A aplicação consome a API REST do FleetManager backend e fornece interface completa para gerenciamento de frotas.

### Tecnologias Principais

- **Angular 18**: Framework principal
- **PrimeNG 17**: Biblioteca de componentes UI
- **PrimeIcons**: Ícones
- **RxJS**: Programação reativa
- **TypeScript 5**: Linguagem
- **SCSS**: Estilização

## Arquitetura

### Estrutura de Diretórios

```
src/
├── app/
│   ├── core/                          # Módulo core (singleton)
│   │   ├── services/                  # Serviços globais
│   │   │   ├── api.service.ts         # Cliente HTTP base
│   │   │   ├── vehicle.service.ts     # Serviço de veículos
│   │   │   ├── driver.service.ts      # Serviço de condutores
│   │   │   ├── trip.service.ts        # Serviço de viagens
│   │   │   ├── maintenance.service.ts # Serviço de manutenções
│   │   │   └── toast.service.ts       # Serviço de notificações
│   │   ├── interceptors/              # HTTP Interceptors
│   │   │   ├── error.interceptor.ts   # Tratamento de erros
│   │   │   └── loading.interceptor.ts # Loading state
│   │   ├── guards/                    # Route guards
│   │   └── models/                    # Interfaces e tipos
│   │       ├── vehicle.model.ts
│   │       ├── driver.model.ts
│   │       ├── trip.model.ts
│   │       └── maintenance.model.ts
│   │
│   ├── shared/                        # Módulo compartilhado
│   │   ├── components/                # Componentes Atomic Design
│   │   │   ├── atoms/                 # Componentes básicos
│   │   │   │   ├── button/
│   │   │   │   ├── input/
│   │   │   │   ├── badge/
│   │   │   │   └── icon/
│   │   │   ├── molecules/             # Combinações simples
│   │   │   │   ├── form-field/
│   │   │   │   ├── search-box/
│   │   │   │   └── status-badge/
│   │   │   └── organisms/             # Componentes complexos
│   │   │       ├── data-table/
│   │   │       ├── form-dialog/
│   │   │       ├── confirm-dialog/
│   │   │       └── stats-card/
│   │   ├── pipes/                     # Pipes customizados
│   │   │   ├── date-format.pipe.ts
│   │   │   ├── currency-format.pipe.ts
│   │   │   ├── phone-format.pipe.ts
│   │   │   └── vehicle-status.pipe.ts
│   │   └── directives/                # Diretivas customizadas
│   │
│   ├── features/                      # Módulos de funcionalidades
│   │   ├── dashboard/
│   │   │   ├── pages/
│   │   │   │   └── dashboard-page/
│   │   │   ├── components/
│   │   │   │   ├── fleet-summary/
│   │   │   │   ├── upcoming-maintenance/
│   │   │   │   └── active-trips/
│   │   │   └── dashboard.module.ts
│   │   │
│   │   ├── vehicles/
│   │   │   ├── pages/
│   │   │   │   └── vehicles-page/
│   │   │   ├── components/
│   │   │   │   ├── vehicle-list/
│   │   │   │   ├── vehicle-form/
│   │   │   │   └── vehicle-details/
│   │   │   └── vehicles.module.ts
│   │   │
│   │   ├── drivers/
│   │   │   ├── pages/
│   │   │   │   └── drivers-page/
│   │   │   ├── components/
│   │   │   │   ├── driver-list/
│   │   │   │   └── driver-form/
│   │   │   └── drivers.module.ts
│   │   │
│   │   ├── trips/
│   │   │   ├── pages/
│   │   │   │   └── trips-page/
│   │   │   ├── components/
│   │   │   │   ├── trip-list/
│   │   │   │   ├── start-trip-form/
│   │   │   │   └── end-trip-form/
│   │   │   └── trips.module.ts
│   │   │
│   │   └── maintenance/
│   │       ├── pages/
│   │       │   └── maintenance-page/
│   │       ├── components/
│   │       │   ├── maintenance-list/
│   │       │   ├── maintenance-form/
│   │       │   └── maintenance-history/
│   │       └── maintenance.module.ts
│   │
│   ├── layout/                        # Templates de layout
│   │   ├── main-layout/
│   │   │   ├── main-layout.component.ts
│   │   │   ├── header/
│   │   │   └── sidebar/
│   │   └── layout.module.ts
│   │
│   ├── app.component.ts
│   ├── app.routes.ts
│   └── app.config.ts
│
├── assets/
│   ├── images/
│   └── i18n/
│
├── styles/
│   ├── _variables.scss               # Variáveis SCSS
│   ├── _mixins.scss                  # Mixins reutilizáveis
│   └── styles.scss                   # Estilos globais
│
└── environments/
    ├── environment.ts
    └── environment.prod.ts
```


## Componentes e Interfaces

### Atomic Design - Hierarquia de Componentes

#### Átomos (Atoms)

Componentes básicos e indivisíveis que servem como blocos de construção.

**Button Component**
```typescript
@Component({
  selector: 'app-button',
  template: `
    <p-button 
      [label]="label"
      [icon]="icon"
      [severity]="severity"
      [disabled]="disabled"
      [loading]="loading"
      (onClick)="handleClick()">
    </p-button>
  `
})
export class ButtonComponent {
  @Input() label: string;
  @Input() icon?: string;
  @Input() severity: 'primary' | 'secondary' | 'success' | 'danger' = 'primary';
  @Input() disabled = false;
  @Input() loading = false;
  @Output() clicked = new EventEmitter<void>();
}
```

**Badge Component**
```typescript
@Component({
  selector: 'app-badge',
  template: `
    <p-tag 
      [value]="label"
      [severity]="severity"
      [icon]="icon">
    </p-tag>
  `
})
export class BadgeComponent {
  @Input() label: string;
  @Input() severity: 'success' | 'info' | 'warning' | 'danger';
  @Input() icon?: string;
}
```

#### Moléculas (Molecules)

Combinações simples de átomos que formam componentes funcionais.

**Form Field Component**
```typescript
@Component({
  selector: 'app-form-field',
  template: `
    <div class="form-field">
      <label [for]="id">{{ label }} <span *ngIf="required">*</span></label>
      <ng-content></ng-content>
      <small *ngIf="error" class="p-error">{{ error }}</small>
    </div>
  `
})
export class FormFieldComponent {
  @Input() id: string;
  @Input() label: string;
  @Input() required = false;
  @Input() error?: string;
}
```

**Search Box Component**
```typescript
@Component({
  selector: 'app-search-box',
  template: `
    <span class="p-input-icon-left">
      <i class="pi pi-search"></i>
      <input 
        pInputText 
        type="text"
        [placeholder]="placeholder"
        [(ngModel)]="searchTerm"
        (ngModelChange)="onSearch()">
    </span>
  `
})
export class SearchBoxComponent {
  @Input() placeholder = 'Buscar...';
  @Output() search = new EventEmitter<string>();
  searchTerm = '';
}
```

**Status Badge Component**
```typescript
@Component({
  selector: 'app-status-badge',
  template: `
    <app-badge 
      [label]="statusLabel"
      [severity]="statusSeverity"
      [icon]="statusIcon">
    </app-badge>
  `
})
export class StatusBadgeComponent {
  @Input() status: VehicleStatus | 'active' | 'inactive';
  
  get statusLabel(): string {
    const labels = {
      'Available': 'Disponível',
      'InUse': 'Em Uso',
      'InMaintenance': 'Em Manutenção',
      'active': 'Ativo',
      'inactive': 'Inativo'
    };
    return labels[this.status];
  }
  
  get statusSeverity(): string {
    const severities = {
      'Available': 'success',
      'InUse': 'info',
      'InMaintenance': 'warning',
      'active': 'success',
      'inactive': 'secondary'
    };
    return severities[this.status];
  }
}
```


#### Organismos (Organisms)

Componentes complexos que combinam moléculas e átomos.

**Data Table Component**
```typescript
@Component({
  selector: 'app-data-table',
  template: `
    <div class="data-table-container">
      <div class="table-header">
        <app-search-box 
          [placeholder]="searchPlaceholder"
          (search)="onSearch($event)">
        </app-search-box>
        <app-button 
          [label]="addButtonLabel"
          icon="pi pi-plus"
          (clicked)="onAdd()">
        </app-button>
      </div>
      
      <p-table 
        [value]="data"
        [loading]="loading"
        [paginator]="true"
        [rows]="10"
        [globalFilterFields]="filterFields">
        <ng-content></ng-content>
      </p-table>
    </div>
  `
})
export class DataTableComponent {
  @Input() data: any[] = [];
  @Input() loading = false;
  @Input() searchPlaceholder = 'Buscar...';
  @Input() addButtonLabel = 'Adicionar';
  @Input() filterFields: string[] = [];
  @Output() add = new EventEmitter<void>();
  @Output() searchChange = new EventEmitter<string>();
}
```

**Form Dialog Component**
```typescript
@Component({
  selector: 'app-form-dialog',
  template: `
    <p-dialog 
      [header]="title"
      [(visible)]="visible"
      [modal]="true"
      [style]="{width: '500px'}"
      (onHide)="onCancel()">
      
      <ng-content></ng-content>
      
      <ng-template pTemplate="footer">
        <app-button 
          label="Cancelar"
          severity="secondary"
          (clicked)="onCancel()">
        </app-button>
        <app-button 
          label="Salvar"
          [loading]="loading"
          [disabled]="!isValid"
          (clicked)="onSave()">
        </app-button>
      </ng-template>
    </p-dialog>
  `
})
export class FormDialogComponent {
  @Input() title: string;
  @Input() visible = false;
  @Input() loading = false;
  @Input() isValid = true;
  @Output() save = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
}
```

**Stats Card Component**
```typescript
@Component({
  selector: 'app-stats-card',
  template: `
    <p-card class="stats-card" [ngClass]="'stats-card--' + color">
      <div class="stats-card__content">
        <div class="stats-card__icon">
          <i [class]="icon"></i>
        </div>
        <div class="stats-card__info">
          <h3 class="stats-card__value">{{ value }}</h3>
          <p class="stats-card__label">{{ label }}</p>
        </div>
      </div>
    </p-card>
  `
})
export class StatsCardComponent {
  @Input() icon: string;
  @Input() value: number | string;
  @Input() label: string;
  @Input() color: 'primary' | 'success' | 'warning' | 'danger' = 'primary';
}
```


## Modelos de Dados

### Interfaces TypeScript

```typescript
// vehicle.model.ts
export interface Vehicle {
  id: string;
  plate: string;
  model: string;
  year: number;
  mileage: number;
  lastMaintenanceDate: Date;
  nextMaintenanceDate: Date;
  status: VehicleStatus;
}

export enum VehicleStatus {
  Available = 'Available',
  InUse = 'InUse',
  InMaintenance = 'InMaintenance'
}

export interface CreateVehicleRequest {
  plate: string;
  model: string;
  year: number;
  mileage: number;
}

export interface UpdateVehicleRequest {
  model: string;
  year: number;
  mileage: number;
}

// driver.model.ts
export interface Driver {
  id: string;
  name: string;
  licenseNumber: string;
  phone: string;
  active: boolean;
}

export interface CreateDriverRequest {
  name: string;
  licenseNumber: string;
  phone: string;
}

export interface UpdateDriverRequest {
  name: string;
  licenseNumber: string;
  phone: string;
}

// trip.model.ts
export interface Trip {
  id: string;
  vehicleId: string;
  driverId: string;
  route: string;
  startDate: Date;
  endDate?: Date;
  distance: number;
}

export interface StartTripRequest {
  vehicleId: string;
  driverId: string;
  route: string;
}

export interface EndTripRequest {
  distance: number;
}

// maintenance.model.ts
export interface Maintenance {
  id: string;
  vehicleId: string;
  date: Date;
  description: string;
  cost: number;
}

export interface CreateMaintenanceRequest {
  vehicleId: string;
  date: Date;
  description: string;
  cost: number;
}

// dashboard.model.ts
export interface DashboardStats {
  availableVehicles: number;
  inUseVehicles: number;
  inMaintenanceVehicles: number;
  activeTrips: number;
  upcomingMaintenance: Vehicle[];
}
```

## Serviços

### API Service (Base)

```typescript
@Injectable({ providedIn: 'root' })
export class ApiService {
  private readonly baseUrl = environment.apiUrl;
  
  constructor(private http: HttpClient) {}
  
  get<T>(endpoint: string, params?: HttpParams): Observable<T> {
    return this.http.get<T>(`${this.baseUrl}${endpoint}`, { params });
  }
  
  post<T>(endpoint: string, body: any): Observable<T> {
    return this.http.post<T>(`${this.baseUrl}${endpoint}`, body);
  }
  
  put<T>(endpoint: string, body: any): Observable<T> {
    return this.http.put<T>(`${this.baseUrl}${endpoint}`, body);
  }
  
  delete<T>(endpoint: string): Observable<T> {
    return this.http.delete<T>(`${this.baseUrl}${endpoint}`);
  }
}
```

### Vehicle Service

```typescript
@Injectable({ providedIn: 'root' })
export class VehicleService {
  private readonly endpoint = '/api/vehicles';
  
  constructor(private api: ApiService) {}
  
  getAll(): Observable<Vehicle[]> {
    return this.api.get<Vehicle[]>(this.endpoint);
  }
  
  getById(id: string): Observable<Vehicle> {
    return this.api.get<Vehicle>(`${this.endpoint}/${id}`);
  }
  
  getAvailable(): Observable<Vehicle[]> {
    return this.api.get<Vehicle[]>(`${this.endpoint}/available`);
  }
  
  create(request: CreateVehicleRequest): Observable<Vehicle> {
    return this.api.post<Vehicle>(this.endpoint, request);
  }
  
  update(id: string, request: UpdateVehicleRequest): Observable<Vehicle> {
    return this.api.put<Vehicle>(`${this.endpoint}/${id}`, request);
  }
  
  delete(id: string): Observable<void> {
    return this.api.delete<void>(`${this.endpoint}/${id}`);
  }
}
```

### Toast Service

```typescript
@Injectable({ providedIn: 'root' })
export class ToastService {
  constructor(private messageService: MessageService) {}
  
  success(message: string, detail?: string): void {
    this.messageService.add({
      severity: 'success',
      summary: message,
      detail,
      life: 3000
    });
  }
  
  error(message: string, detail?: string): void {
    this.messageService.add({
      severity: 'error',
      summary: message,
      detail,
      life: 5000
    });
  }
  
  info(message: string, detail?: string): void {
    this.messageService.add({
      severity: 'info',
      summary: message,
      detail,
      life: 3000
    });
  }
  
  warning(message: string, detail?: string): void {
    this.messageService.add({
      severity: 'warn',
      summary: message,
      detail,
      life: 4000
    });
  }
}
```


## Interceptors

### Error Interceptor

```typescript
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(private toast: ToastService) {}
  
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Ocorreu um erro inesperado';
        
        if (error.error?.error) {
          errorMessage = error.error.error;
        } else if (error.status === 0) {
          errorMessage = 'Sem conexão com internet. Verifique sua rede';
        } else if (error.status === 404) {
          errorMessage = 'Recurso não encontrado';
        } else if (error.status === 500) {
          errorMessage = 'Erro interno do servidor. Contate o suporte';
        } else if (error.status === 409) {
          errorMessage = error.error?.error || 'Recurso já existe';
        }
        
        this.toast.error('Erro', errorMessage);
        return throwError(() => error);
      })
    );
  }
}
```

### Loading Interceptor

```typescript
@Injectable()
export class LoadingInterceptor implements HttpInterceptor {
  private loadingRequests = 0;
  
  constructor(private loadingService: LoadingService) {}
  
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.loadingRequests++;
    this.loadingService.show();
    
    return next.handle(req).pipe(
      finalize(() => {
        this.loadingRequests--;
        if (this.loadingRequests === 0) {
          this.loadingService.hide();
        }
      })
    );
  }
}
```

## Rotas

```typescript
export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
      },
      {
        path: 'dashboard',
        loadChildren: () => import('./features/dashboard/dashboard.module')
          .then(m => m.DashboardModule)
      },
      {
        path: 'vehicles',
        loadChildren: () => import('./features/vehicles/vehicles.module')
          .then(m => m.VehiclesModule)
      },
      {
        path: 'drivers',
        loadChildren: () => import('./features/drivers/drivers.module')
          .then(m => m.DriversModule)
      },
      {
        path: 'trips',
        loadChildren: () => import('./features/trips/trips.module')
          .then(m => m.TripsModule)
      },
      {
        path: 'maintenance',
        loadChildren: () => import('./features/maintenance/maintenance.module')
          .then(m => m.MaintenanceModule)
      }
    ]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
```

## Layout

### Main Layout Component

```typescript
@Component({
  selector: 'app-main-layout',
  template: `
    <div class="layout-wrapper">
      <app-sidebar 
        [menuItems]="menuItems"
        [(collapsed)]="sidebarCollapsed">
      </app-sidebar>
      
      <div class="layout-main" [ngClass]="{'layout-main--expanded': sidebarCollapsed}">
        <app-header 
          [title]="currentPageTitle"
          (menuToggle)="toggleSidebar()">
        </app-header>
        
        <main class="layout-content">
          <router-outlet></router-outlet>
        </main>
      </div>
    </div>
    
    <p-toast position="top-right"></p-toast>
    <p-confirmDialog></p-confirmDialog>
  `
})
export class MainLayoutComponent {
  sidebarCollapsed = false;
  currentPageTitle = 'Dashboard';
  
  menuItems: MenuItem[] = [
    {
      label: 'Dashboard',
      icon: 'pi pi-home',
      routerLink: '/dashboard'
    },
    {
      label: 'Veículos',
      icon: 'pi pi-car',
      routerLink: '/vehicles'
    },
    {
      label: 'Condutores',
      icon: 'pi pi-users',
      routerLink: '/drivers'
    },
    {
      label: 'Viagens',
      icon: 'pi pi-map',
      routerLink: '/trips'
    },
    {
      label: 'Manutenções',
      icon: 'pi pi-wrench',
      routerLink: '/maintenance'
    }
  ];
}
```


## Páginas (Pages)

### Dashboard Page

```typescript
@Component({
  selector: 'app-dashboard-page',
  template: `
    <div class="dashboard">
      <h1>Dashboard</h1>
      
      <div class="dashboard__stats">
        <app-stats-card
          icon="pi pi-check-circle"
          [value]="stats.availableVehicles"
          label="Veículos Disponíveis"
          color="success">
        </app-stats-card>
        
        <app-stats-card
          icon="pi pi-car"
          [value]="stats.inUseVehicles"
          label="Veículos em Uso"
          color="primary">
        </app-stats-card>
        
        <app-stats-card
          icon="pi pi-wrench"
          [value]="stats.inMaintenanceVehicles"
          label="Em Manutenção"
          color="warning">
        </app-stats-card>
        
        <app-stats-card
          icon="pi pi-map"
          [value]="stats.activeTrips"
          label="Viagens Ativas"
          color="primary">
        </app-stats-card>
      </div>
      
      <div class="dashboard__content">
        <div class="dashboard__section">
          <h2>Manutenções Próximas</h2>
          <app-upcoming-maintenance 
            [vehicles]="upcomingMaintenance">
          </app-upcoming-maintenance>
        </div>
        
        <div class="dashboard__section">
          <h2>Viagens Ativas</h2>
          <app-active-trips 
            [trips]="activeTrips">
          </app-active-trips>
        </div>
      </div>
    </div>
  `
})
export class DashboardPageComponent implements OnInit {
  stats: DashboardStats;
  upcomingMaintenance: Vehicle[] = [];
  activeTrips: Trip[] = [];
  
  constructor(
    private vehicleService: VehicleService,
    private tripService: TripService,
    private maintenanceService: MaintenanceService
  ) {}
  
  ngOnInit(): void {
    this.loadDashboardData();
  }
  
  private loadDashboardData(): void {
    forkJoin({
      vehicles: this.vehicleService.getAll(),
      trips: this.tripService.getActive(),
      upcoming: this.maintenanceService.getUpcoming(7)
    }).subscribe({
      next: (data) => {
        this.calculateStats(data.vehicles);
        this.activeTrips = data.trips;
        this.upcomingMaintenance = data.upcoming;
      }
    });
  }
}
```

### Vehicles Page

```typescript
@Component({
  selector: 'app-vehicles-page',
  template: `
    <div class="vehicles-page">
      <h1>Gestão de Veículos</h1>
      
      <app-data-table
        [data]="filteredVehicles"
        [loading]="loading"
        searchPlaceholder="Buscar por placa ou modelo..."
        addButtonLabel="Novo Veículo"
        [filterFields]="['plate', 'model']"
        (add)="openCreateDialog()"
        (searchChange)="onSearch($event)">
        
        <ng-template pTemplate="header">
          <tr>
            <th>Placa</th>
            <th>Modelo</th>
            <th>Ano</th>
            <th>Quilometragem</th>
            <th>Próxima Manutenção</th>
            <th>Status</th>
            <th>Ações</th>
          </tr>
        </ng-template>
        
        <ng-template pTemplate="body" let-vehicle>
          <tr>
            <td>{{ vehicle.plate }}</td>
            <td>{{ vehicle.model }}</td>
            <td>{{ vehicle.year }}</td>
            <td>{{ vehicle.mileage | number }} km</td>
            <td>{{ vehicle.nextMaintenanceDate | date:'dd/MM/yyyy' }}</td>
            <td>
              <app-status-badge [status]="vehicle.status"></app-status-badge>
            </td>
            <td>
              <app-button
                icon="pi pi-pencil"
                severity="secondary"
                (clicked)="openEditDialog(vehicle)">
              </app-button>
              <app-button
                icon="pi pi-trash"
                severity="danger"
                (clicked)="confirmDelete(vehicle)">
              </app-button>
            </td>
          </tr>
        </ng-template>
      </app-data-table>
      
      <app-vehicle-form
        [(visible)]="showDialog"
        [vehicle]="selectedVehicle"
        (save)="onSave($event)"
        (cancel)="closeDialog()">
      </app-vehicle-form>
    </div>
  `
})
export class VehiclesPageComponent implements OnInit {
  vehicles: Vehicle[] = [];
  filteredVehicles: Vehicle[] = [];
  loading = false;
  showDialog = false;
  selectedVehicle?: Vehicle;
  
  constructor(
    private vehicleService: VehicleService,
    private toast: ToastService,
    private confirmationService: ConfirmationService
  ) {}
  
  ngOnInit(): void {
    this.loadVehicles();
  }
  
  private loadVehicles(): void {
    this.loading = true;
    this.vehicleService.getAll().subscribe({
      next: (vehicles) => {
        this.vehicles = vehicles;
        this.filteredVehicles = vehicles;
        this.loading = false;
      }
    });
  }
  
  onSearch(term: string): void {
    this.filteredVehicles = this.vehicles.filter(v =>
      v.plate.toLowerCase().includes(term.toLowerCase()) ||
      v.model.toLowerCase().includes(term.toLowerCase())
    );
  }
  
  openCreateDialog(): void {
    this.selectedVehicle = undefined;
    this.showDialog = true;
  }
  
  openEditDialog(vehicle: Vehicle): void {
    this.selectedVehicle = vehicle;
    this.showDialog = true;
  }
  
  onSave(vehicle: Vehicle): void {
    this.showDialog = false;
    this.loadVehicles();
    this.toast.success('Sucesso', 'Veículo salvo com sucesso');
  }
  
  confirmDelete(vehicle: Vehicle): void {
    this.confirmationService.confirm({
      message: `Deseja realmente excluir o veículo ${vehicle.plate}?`,
      header: 'Confirmar Exclusão',
      icon: 'pi pi-exclamation-triangle',
      accept: () => this.deleteVehicle(vehicle.id)
    });
  }
  
  private deleteVehicle(id: string): void {
    this.vehicleService.delete(id).subscribe({
      next: () => {
        this.loadVehicles();
        this.toast.success('Sucesso', 'Veículo excluído com sucesso');
      }
    });
  }
}
```


## Formulários

### Vehicle Form Component

```typescript
@Component({
  selector: 'app-vehicle-form',
  template: `
    <app-form-dialog
      [title]="isEditMode ? 'Editar Veículo' : 'Novo Veículo'"
      [(visible)]="visible"
      [loading]="loading"
      [isValid]="form.valid"
      (save)="onSubmit()"
      (cancel)="onCancel()">
      
      <form [formGroup]="form">
        <app-form-field
          id="plate"
          label="Placa"
          [required]="true"
          [error]="getFieldError('plate')">
          <input
            pInputText
            id="plate"
            formControlName="plate"
            [disabled]="isEditMode"
            placeholder="ABC1234">
        </app-form-field>
        
        <app-form-field
          id="model"
          label="Modelo"
          [required]="true"
          [error]="getFieldError('model')">
          <input
            pInputText
            id="model"
            formControlName="model"
            placeholder="Toyota Corolla">
        </app-form-field>
        
        <app-form-field
          id="year"
          label="Ano"
          [required]="true"
          [error]="getFieldError('year')">
          <p-inputNumber
            id="year"
            formControlName="year"
            [useGrouping]="false"
            [min]="1900"
            [max]="currentYear + 1">
          </p-inputNumber>
        </app-form-field>
        
        <app-form-field
          id="mileage"
          label="Quilometragem"
          [required]="true"
          [error]="getFieldError('mileage')">
          <p-inputNumber
            id="mileage"
            formControlName="mileage"
            suffix=" km"
            [min]="0">
          </p-inputNumber>
        </app-form-field>
      </form>
    </app-form-dialog>
  `
})
export class VehicleFormComponent implements OnInit, OnChanges {
  @Input() visible = false;
  @Input() vehicle?: Vehicle;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<Vehicle>();
  @Output() cancel = new EventEmitter<void>();
  
  form: FormGroup;
  loading = false;
  currentYear = new Date().getFullYear();
  
  constructor(
    private fb: FormBuilder,
    private vehicleService: VehicleService
  ) {
    this.form = this.createForm();
  }
  
  get isEditMode(): boolean {
    return !!this.vehicle;
  }
  
  ngOnInit(): void {
    this.form = this.createForm();
  }
  
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['vehicle'] && this.vehicle) {
      this.form.patchValue(this.vehicle);
    } else if (changes['visible'] && this.visible && !this.vehicle) {
      this.form.reset();
    }
  }
  
  private createForm(): FormGroup {
    return this.fb.group({
      plate: ['', [Validators.required, Validators.pattern(/^[A-Z]{3}[0-9]{4}$/)]],
      model: ['', Validators.required],
      year: [this.currentYear, [Validators.required, Validators.min(1900), Validators.max(this.currentYear + 1)]],
      mileage: [0, [Validators.required, Validators.min(0)]]
    });
  }
  
  getFieldError(fieldName: string): string | undefined {
    const field = this.form.get(fieldName);
    if (field?.invalid && field?.touched) {
      if (field.errors?.['required']) {
        return 'Campo obrigatório';
      }
      if (field.errors?.['pattern']) {
        return 'Formato inválido (ex: ABC1234)';
      }
      if (field.errors?.['min']) {
        return `Valor mínimo: ${field.errors['min'].min}`;
      }
      if (field.errors?.['max']) {
        return `Valor máximo: ${field.errors['max'].max}`;
      }
    }
    return undefined;
  }
  
  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    
    this.loading = true;
    const request = this.form.value;
    
    const operation = this.isEditMode
      ? this.vehicleService.update(this.vehicle!.id, request)
      : this.vehicleService.create(request);
    
    operation.subscribe({
      next: (vehicle) => {
        this.loading = false;
        this.save.emit(vehicle);
      },
      error: () => {
        this.loading = false;
      }
    });
  }
  
  onCancel(): void {
    this.form.reset();
    this.cancel.emit();
    this.visibleChange.emit(false);
  }
}
```


## Pipes Customizados

### Date Format Pipe

```typescript
@Pipe({ name: 'dateFormat' })
export class DateFormatPipe implements PipeTransform {
  transform(value: Date | string, format: string = 'dd/MM/yyyy HH:mm'): string {
    if (!value) return '';
    const date = typeof value === 'string' ? new Date(value) : value;
    return formatDate(date, format, 'pt-BR');
  }
}
```

### Currency Format Pipe

```typescript
@Pipe({ name: 'currencyFormat' })
export class CurrencyFormatPipe implements PipeTransform {
  transform(value: number): string {
    if (value === null || value === undefined) return '';
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }
}
```

### Phone Format Pipe

```typescript
@Pipe({ name: 'phoneFormat' })
export class PhoneFormatPipe implements PipeTransform {
  transform(value: string): string {
    if (!value) return '';
    // +5511999999999 -> +55 (11) 99999-9999
    const cleaned = value.replace(/\D/g, '');
    if (cleaned.length === 13) {
      return `+${cleaned.slice(0, 2)} (${cleaned.slice(2, 4)}) ${cleaned.slice(4, 9)}-${cleaned.slice(9)}`;
    }
    return value;
  }
}
```

### Vehicle Status Pipe

```typescript
@Pipe({ name: 'vehicleStatus' })
export class VehicleStatusPipe implements PipeTransform {
  transform(value: VehicleStatus): string {
    const statusMap = {
      [VehicleStatus.Available]: 'Disponível',
      [VehicleStatus.InUse]: 'Em Uso',
      [VehicleStatus.InMaintenance]: 'Em Manutenção'
    };
    return statusMap[value] || value;
  }
}
```

## Tratamento de Erros

### Error Handler Strategy

```typescript
@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private injector: Injector) {}
  
  handleError(error: Error | HttpErrorResponse): void {
    const toast = this.injector.get(ToastService);
    
    if (error instanceof HttpErrorResponse) {
      // Erros HTTP são tratados pelo interceptor
      console.error('HTTP Error:', error);
    } else {
      // Erros de aplicação
      console.error('Application Error:', error);
      toast.error('Erro', 'Ocorreu um erro inesperado na aplicação');
    }
  }
}
```

## Estratégia de Testes

### Testes Unitários

- **Componentes**: Testar lógica de negócio, interações e emissão de eventos
- **Serviços**: Testar chamadas HTTP e transformação de dados
- **Pipes**: Testar formatação de dados
- **Interceptors**: Testar tratamento de erros e loading

### Testes de Integração

- **Fluxos completos**: Testar navegação e interação entre componentes
- **Formulários**: Testar validação e submissão
- **API Integration**: Testar comunicação com backend (usando mocks)

### Ferramentas

- **Jasmine**: Framework de testes
- **Karma**: Test runner
- **Angular Testing Library**: Utilitários de teste

## Estilização

### Variáveis SCSS

```scss
// _variables.scss
$primary-color: #3B82F6;
$success-color: #10B981;
$warning-color: #F59E0B;
$danger-color: #EF4444;
$secondary-color: #6B7280;

$font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
$font-size-base: 14px;
$font-size-large: 16px;
$font-size-small: 12px;

$spacing-xs: 0.25rem;
$spacing-sm: 0.5rem;
$spacing-md: 1rem;
$spacing-lg: 1.5rem;
$spacing-xl: 2rem;

$border-radius: 6px;
$box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);

$breakpoint-mobile: 768px;
$breakpoint-tablet: 1024px;
$breakpoint-desktop: 1280px;
```

### Mixins

```scss
// _mixins.scss
@mixin responsive($breakpoint) {
  @if $breakpoint == mobile {
    @media (max-width: $breakpoint-mobile) { @content; }
  }
  @if $breakpoint == tablet {
    @media (max-width: $breakpoint-tablet) { @content; }
  }
  @if $breakpoint == desktop {
    @media (min-width: $breakpoint-desktop) { @content; }
  }
}

@mixin card {
  background: white;
  border-radius: $border-radius;
  box-shadow: $box-shadow;
  padding: $spacing-lg;
}

@mixin flex-center {
  display: flex;
  align-items: center;
  justify-content: center;
}
```

## Performance

### Estratégias de Otimização

1. **Lazy Loading**: Módulos carregados sob demanda
2. **OnPush Change Detection**: Componentes com estratégia OnPush quando possível
3. **TrackBy Functions**: Em listas para otimizar renderização
4. **Debounce**: Em campos de busca e filtros
5. **Caching**: Dados de veículos disponíveis e condutores ativos
6. **Virtual Scrolling**: Para listas muito grandes (se necessário)

### Bundle Optimization

- Tree shaking automático do Angular
- Lazy loading de módulos
- Importação seletiva de componentes PrimeNG
- Minificação e compressão em produção

## Acessibilidade

### Diretrizes WCAG 2.1

- Labels em todos os campos de formulário
- Contraste adequado de cores (mínimo 4.5:1)
- Navegação por teclado em todos os componentes
- ARIA labels em ícones e botões sem texto
- Mensagens de erro associadas aos campos
- Focus visível em elementos interativos

## Internacionalização

### Configuração pt-BR

```typescript
import { LOCALE_ID } from '@angular/core';
import { registerLocaleData } from '@angular/common';
import localePt from '@angular/common/locales/pt';

registerLocaleData(localePt);

export const appConfig: ApplicationConfig = {
  providers: [
    { provide: LOCALE_ID, useValue: 'pt-BR' },
    // ... outros providers
  ]
};
```

### PrimeNG Locale

```typescript
import { PrimeNGConfig } from 'primeng/api';

export class AppComponent implements OnInit {
  constructor(private config: PrimeNGConfig) {}
  
  ngOnInit() {
    this.config.setTranslation({
      dayNames: ['Domingo', 'Segunda', 'Terça', 'Quarta', 'Quinta', 'Sexta', 'Sábado'],
      dayNamesShort: ['Dom', 'Seg', 'Ter', 'Qua', 'Qui', 'Sex', 'Sáb'],
      dayNamesMin: ['D', 'S', 'T', 'Q', 'Q', 'S', 'S'],
      monthNames: ['Janeiro', 'Fevereiro', 'Março', 'Abril', 'Maio', 'Junho', 
                   'Julho', 'Agosto', 'Setembro', 'Outubro', 'Novembro', 'Dezembro'],
      monthNamesShort: ['Jan', 'Fev', 'Mar', 'Abr', 'Mai', 'Jun', 
                        'Jul', 'Ago', 'Set', 'Out', 'Nov', 'Dez'],
      today: 'Hoje',
      clear: 'Limpar',
      accept: 'Sim',
      reject: 'Não'
    });
  }
}
```
