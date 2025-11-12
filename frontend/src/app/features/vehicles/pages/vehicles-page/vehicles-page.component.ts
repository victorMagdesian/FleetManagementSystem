import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { ConfirmationService } from 'primeng/api';
import { VehicleService, ToastService } from '../../../../core/services';
import { Vehicle } from '../../../../core/models';
import { StatusBadgeComponent } from '../../../../shared/components/molecules/status-badge/status-badge.component';
import { VehicleFormComponent } from '../../components/vehicle-form/vehicle-form.component';
import { DateFormatPipe } from '../../../../shared/pipes';

@Component({
  selector: 'app-vehicles-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    StatusBadgeComponent,
    VehicleFormComponent,
    DateFormatPipe
  ],
  templateUrl: './vehicles-page.component.html',
  styleUrls: ['./vehicles-page.component.scss']
})
export class VehiclesPageComponent implements OnInit {
  vehicles: Vehicle[] = [];
  filteredVehicles: Vehicle[] = [];
  loading = false;
  searchTerm = '';
  showFormDialog = false;
  selectedVehicle: Vehicle | null = null;

  constructor(
    private vehicleService: VehicleService,
    private toastService: ToastService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadVehicles();
  }

  loadVehicles(): void {
    this.loading = true;
    this.vehicleService.getAll().subscribe({
      next: (vehicles) => {
        this.vehicles = vehicles;
        this.applyFilter();
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao carregar veículos');
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    if (!this.searchTerm) {
      this.filteredVehicles = [...this.vehicles];
      return;
    }

    const term = this.searchTerm.toLowerCase();
    this.filteredVehicles = this.vehicles.filter(vehicle =>
      vehicle.plate.toLowerCase().includes(term) ||
      vehicle.model.toLowerCase().includes(term)
    );
  }

  onSearch(): void {
    this.applyFilter();
  }

  openCreateDialog(): void {
    this.selectedVehicle = null;
    this.showFormDialog = true;
  }

  openEditDialog(vehicle: Vehicle): void {
    this.selectedVehicle = vehicle;
    this.showFormDialog = true;
  }

  onFormClose(): void {
    this.showFormDialog = false;
    this.selectedVehicle = null;
  }

  onFormSuccess(): void {
    this.showFormDialog = false;
    this.selectedVehicle = null;
    this.loadVehicles();
  }

  confirmDelete(vehicle: Vehicle): void {
    this.confirmationService.confirm({
      message: `Tem certeza que deseja excluir o veículo ${vehicle.plate}?`,
      header: 'Confirmar Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim',
      rejectLabel: 'Não',
      accept: () => {
        this.deleteVehicle(vehicle.id);
      }
    });
  }

  private deleteVehicle(id: string): void {
    this.vehicleService.delete(id).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Veículo excluído com sucesso');
        this.loadVehicles();
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao excluir veículo');
      }
    });
  }
}
