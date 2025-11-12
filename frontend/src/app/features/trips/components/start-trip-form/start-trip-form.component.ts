import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DropdownModule } from 'primeng/dropdown';
import { TripService, VehicleService, DriverService, ToastService } from '../../../../core/services';
import { Vehicle, Driver, StartTripRequest } from '../../../../core/models';

@Component({
  selector: 'app-start-trip-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    DropdownModule
  ],
  templateUrl: './start-trip-form.component.html',
  styleUrls: ['./start-trip-form.component.scss']
})
export class StartTripFormComponent implements OnChanges {
  @Input() visible = false;
  @Output() onClose = new EventEmitter<void>();
  @Output() onSuccess = new EventEmitter<void>();

  form: FormGroup;
  loading = false;
  loadingData = false;
  availableVehicles: Vehicle[] = [];
  activeDrivers: Driver[] = [];

  constructor(
    private fb: FormBuilder,
    private tripService: TripService,
    private vehicleService: VehicleService,
    private driverService: DriverService,
    private toastService: ToastService
  ) {
    this.form = this.createForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && this.visible) {
      this.resetForm();
      this.loadFormData();
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      vehicleId: [null, Validators.required],
      driverId: [null, Validators.required],
      route: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(200)
      ]]
    });
  }

  private resetForm(): void {
    this.form.reset();
  }

  private loadFormData(): void {
    this.loadingData = true;
    
    this.vehicleService.getAvailable().subscribe({
      next: (vehicles) => {
        this.availableVehicles = vehicles;
        this.checkDataLoaded();
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao carregar veículos disponíveis');
        this.loadingData = false;
      }
    });

    this.driverService.getActive().subscribe({
      next: (drivers) => {
        this.activeDrivers = drivers;
        this.checkDataLoaded();
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao carregar condutores ativos');
        this.loadingData = false;
      }
    });
  }

  private checkDataLoaded(): void {
    if (this.availableVehicles.length > 0 && this.activeDrivers.length > 0) {
      this.loadingData = false;
    }
  }

  get vehicleIdControl() {
    return this.form.get('vehicleId');
  }

  get driverIdControl() {
    return this.form.get('driverId');
  }

  get routeControl() {
    return this.form.get('route');
  }

  getErrorMessage(controlName: string): string {
    const control = this.form.get(controlName);
    
    if (!control || !control.errors || !(control.dirty || control.touched)) {
      return '';
    }

    if (control.hasError('required')) {
      return 'Campo obrigatório';
    }

    if (controlName === 'route') {
      if (control.hasError('minlength')) {
        return 'Mínimo de 3 caracteres';
      }
      if (control.hasError('maxlength')) {
        return 'Máximo de 200 caracteres';
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
    const request: StartTripRequest = this.form.value;
    
    this.tripService.start(request).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Viagem iniciada com sucesso');
        this.loading = false;
        this.onSuccess.emit();
      },
      error: (error) => {
        const message = error?.error?.error || 'Falha ao iniciar viagem';
        this.toastService.error('Erro', message);
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.onClose.emit();
  }
}
