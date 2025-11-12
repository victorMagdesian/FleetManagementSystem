import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { TagModule } from 'primeng/tag';
import { ConfirmationService } from 'primeng/api';
import { DriverService, ToastService } from '../../../../core/services';
import { Driver } from '../../../../core/models';
import { DriverFormComponent } from '../../components/driver-form/driver-form.component';
import { PhoneFormatPipe } from '../../../../shared/pipes';

@Component({
  selector: 'app-drivers-page',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    TagModule,
    DriverFormComponent,
    PhoneFormatPipe
  ],
  templateUrl: './drivers-page.component.html',
  styleUrls: ['./drivers-page.component.scss']
})
export class DriversPageComponent implements OnInit {
  drivers: Driver[] = [];
  filteredDrivers: Driver[] = [];
  loading = false;
  searchTerm = '';
  showFormDialog = false;
  selectedDriver: Driver | null = null;

  constructor(
    private driverService: DriverService,
    private toastService: ToastService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadDrivers();
  }

  loadDrivers(): void {
    this.loading = true;
    this.driverService.getAll().subscribe({
      next: (drivers) => {
        this.drivers = drivers;
        this.applyFilter();
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao carregar condutores');
        this.loading = false;
      }
    });
  }

  applyFilter(): void {
    if (!this.searchTerm) {
      this.filteredDrivers = [...this.drivers];
      return;
    }

    const term = this.searchTerm.toLowerCase();
    this.filteredDrivers = this.drivers.filter(driver =>
      driver.name.toLowerCase().includes(term) ||
      driver.licenseNumber.includes(term)
    );
  }

  onSearch(): void {
    this.applyFilter();
  }

  openCreateDialog(): void {
    this.selectedDriver = null;
    this.showFormDialog = true;
  }

  openEditDialog(driver: Driver): void {
    this.selectedDriver = driver;
    this.showFormDialog = true;
  }

  onFormClose(): void {
    this.showFormDialog = false;
    this.selectedDriver = null;
  }

  onFormSuccess(): void {
    this.showFormDialog = false;
    this.selectedDriver = null;
    this.loadDrivers();
  }

  confirmDelete(driver: Driver): void {
    this.confirmationService.confirm({
      message: `Tem certeza que deseja excluir o condutor ${driver.name}?`,
      header: 'Confirmar Exclusão',
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim',
      rejectLabel: 'Não',
      accept: () => {
        this.deleteDriver(driver.id);
      }
    });
  }

  private deleteDriver(id: string): void {
    this.driverService.delete(id).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Condutor excluído com sucesso');
        this.loadDrivers();
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao excluir condutor');
      }
    });
  }

  toggleDriverStatus(driver: Driver): void {
    const action = driver.active ? 'desativar' : 'ativar';
    const actionCapitalized = driver.active ? 'Desativar' : 'Ativar';

    this.confirmationService.confirm({
      message: `Tem certeza que deseja ${action} o condutor ${driver.name}?`,
      header: `Confirmar ${actionCapitalized}`,
      icon: 'pi pi-exclamation-triangle',
      acceptLabel: 'Sim',
      rejectLabel: 'Não',
      accept: () => {
        if (driver.active) {
          this.deactivateDriver(driver.id);
        } else {
          this.activateDriver(driver.id);
        }
      }
    });
  }

  private activateDriver(id: string): void {
    this.driverService.activate(id).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Condutor ativado com sucesso');
        this.loadDrivers();
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao ativar condutor');
      }
    });
  }

  private deactivateDriver(id: string): void {
    this.driverService.deactivate(id).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Condutor desativado com sucesso');
        this.loadDrivers();
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao desativar condutor');
      }
    });
  }
}
