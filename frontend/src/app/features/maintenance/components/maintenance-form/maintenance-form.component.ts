import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { DropdownModule } from 'primeng/dropdown';
import { CalendarModule } from 'primeng/calendar';
import { MaintenanceService, VehicleService, ToastService } from '../../../../core/services';
import { Vehicle, CreateMaintenanceRequest } from '../../../../core/models';

@Component({
  selector: 'app-maintenance-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule,
    DropdownModule,
    CalendarModule
  ],
  templateUrl: './maintenance-form.component.html',
  styleUrls: ['./maintenance-form.component.scss']
})
export class MaintenanceFormComponent implements OnChanges {
  @Input() visible = false;
  @Output() onClose = new EventEmitter<void>();
  @Output() onSuccess = new EventEmitter<void>();

  form: FormGroup;
  loading = false;
  loadingVehicles = false;
  vehicles: Vehicle[] = [];

  constructor(
    private fb: FormBuilder,
    private maintenanceService: MaintenanceService,
    private vehicleService: VehicleService,
    private toastService: ToastService
  ) {
    this.form = this.createForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && this.visible) {
      this.resetForm();
      this.loadVehicles();
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      vehicleId: [null, Validators.required],
      date: [new Date(), Validators.required],
      description: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(500)
      ]],
      cost: [null, [
        Validators.required,
        Validators.min(0.01)
      ]]
    });
  }

  private resetForm(): void {
    this.form.reset({
      date: new Date()
    });
  }

  private loadVehicles(): void {
    this.loadingVehicles = true;
    this.vehicleService.getAll().subscribe({
      next: (vehicles) => {
        this.vehicles = vehicles;
        this.loadingVehicles = false;
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao carregar veículos');
        this.loadingVehicles = false;
      }
    });
  }

  get vehicleIdControl() {
    return this.form.get('vehicleId');
  }

  get dateControl() {
    return this.form.get('date');
  }

  get descriptionControl() {
    return this.form.get('description');
  }

  get costControl() {
    return this.form.get('cost');
  }

  getErrorMessage(controlName: string): string {
    const control = this.form.get(controlName);
    
    if (!control || !control.errors || !(control.dirty || control.touched)) {
      return '';
    }

    if (control.hasError('required')) {
      return 'Campo obrigatório';
    }

    if (controlName === 'description') {
      if (control.hasError('minlength')) {
        return 'Mínimo de 3 caracteres';
      }
      if (control.hasError('maxlength')) {
        return 'Máximo de 500 caracteres';
      }
    }

    if (controlName === 'cost') {
      if (control.hasError('min')) {
        return 'Custo deve ser maior que zero';
      }
    }

    return '';
  }

  getVehicleLabel(vehicle: Vehicle): string {
    return `${vehicle.plate} - ${vehicle.model}`;
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    const request: CreateMaintenanceRequest = {
      ...this.form.value,
      date: this.form.value.date.toISOString()
    };
    
    this.maintenanceService.create(request).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Manutenção registrada com sucesso');
        this.loading = false;
        this.onSuccess.emit();
      },
      error: (error) => {
        const message = error?.error?.error || 'Falha ao registrar manutenção';
        this.toastService.error('Erro', message);
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.onClose.emit();
  }
}
