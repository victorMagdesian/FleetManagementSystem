import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TabViewModule } from 'primeng/tabview';
import { TooltipModule } from 'primeng/tooltip';
import { MaintenanceService, VehicleService, ToastService } from '../../../../core/services';
import { Maintenance, Vehicle } from '../../../../core/models';
import { MaintenanceFormComponent } from '../../components/maintenance-form/maintenance-form.component';
import { MaintenanceHistoryComponent } from '../../components/maintenance-history/maintenance-history.component';
import { DateFormatPipe, CurrencyFormatPipe } from '../../../../shared/pipes';

@Component({
  selector: 'app-maintenance-page',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    TabViewModule,
    TooltipModule,
    MaintenanceFormComponent,
    MaintenanceHistoryComponent,
    DateFormatPipe,
    CurrencyFormatPipe
  ],
  templateUrl: './maintenance-page.component.html',
  styleUrls: ['./maintenance-page.component.scss']
})
export class MaintenancePageComponent implements OnInit {
  allMaintenance: Maintenance[] = [];
  upcomingVehicles: Vehicle[] = [];
  loading = false;
  showFormDialog = false;
  showHistoryDialog = false;
  selectedVehicleId: string | null = null;

  constructor(
    private maintenanceService: MaintenanceService,
    private vehicleService: VehicleService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loadMaintenance();
    this.loadUpcoming();
  }

  loadMaintenance(): void {
    this.loading = true;
    this.maintenanceService.getAll().subscribe({
      next: (maintenance) => {
        this.allMaintenance = maintenance;
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao carregar manutenções');
        this.loading = false;
      }
    });
  }

  loadUpcoming(): void {
    this.maintenanceService.getUpcoming(7).subscribe({
      next: (vehicles) => {
        this.upcomingVehicles = vehicles;
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao carregar manutenções próximas');
      }
    });
  }

  openCreateDialog(): void {
    this.showFormDialog = true;
  }

  onFormClose(): void {
    this.showFormDialog = false;
  }

  onFormSuccess(): void {
    this.showFormDialog = false;
    this.loadMaintenance();
    this.loadUpcoming();
  }

  openVehicleHistory(vehicleId: string): void {
    this.selectedVehicleId = vehicleId;
    this.showHistoryDialog = true;
  }

  onHistoryClose(): void {
    this.showHistoryDialog = false;
    this.selectedVehicleId = null;
  }
}
