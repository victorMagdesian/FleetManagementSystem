import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputNumberModule } from 'primeng/inputnumber';
import { VehicleService, ToastService } from '../../../../core/services';
import { Vehicle, CreateVehicleRequest, UpdateVehicleRequest } from '../../../../core/models';

@Component({
  selector: 'app-vehicle-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputNumberModule
  ],
  templateUrl: './vehicle-form.component.html',
  styleUrls: ['./vehicle-form.component.scss']
})
export class VehicleFormComponent implements OnChanges {
  @Input() visible = false;
  @Input() vehicle: Vehicle | null = null;
  @Output() onClose = new EventEmitter<void>();
  @Output() onSuccess = new EventEmitter<void>();

  form: FormGroup;
  loading = false;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private vehicleService: VehicleService,
    private toastService: ToastService
  ) {
    this.form = this.createForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['vehicle'] || changes['visible']) {
      if (this.visible) {
        this.isEditMode = !!this.vehicle;
        this.resetForm();
      }
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      plate: ['', [
        Validators.required,
        Validators.pattern(/^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$/)
      ]],
      model: ['', [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(100)
      ]],
      year: [null, [
        Validators.required,
        Validators.min(1900),
        Validators.max(new Date().getFullYear() + 1)
      ]],
      mileage: [null, [
        Validators.required,
        Validators.min(0)
      ]]
    });
  }

  private resetForm(): void {
    if (this.vehicle) {
      this.form.patchValue({
        plate: this.vehicle.plate,
        model: this.vehicle.model,
        year: this.vehicle.year,
        mileage: this.vehicle.mileage
      });
      this.form.get('plate')?.disable();
    } else {
      this.form.reset();
      this.form.get('plate')?.enable();
    }
  }

  get dialogTitle(): string {
    return this.isEditMode ? 'Editar Veículo' : 'Novo Veículo';
  }

  get plateControl() {
    return this.form.get('plate');
  }

  get modelControl() {
    return this.form.get('model');
  }

  get yearControl() {
    return this.form.get('year');
  }

  get mileageControl() {
    return this.form.get('mileage');
  }

  hasError(controlName: string, errorName: string): boolean {
    const control = this.form.get(controlName);
    return !!(control?.hasError(errorName) && (control?.dirty || control?.touched));
  }

  getErrorMessage(controlName: string): string {
    const control = this.form.get(controlName);
    
    if (!control || !control.errors || !(control.dirty || control.touched)) {
      return '';
    }

    if (control.hasError('required')) {
      return 'Campo obrigatório';
    }

    if (controlName === 'plate') {
      if (control.hasError('pattern')) {
        return 'Formato inválido (ex: ABC1234 ou ABC1D23)';
      }
    }

    if (controlName === 'model') {
      if (control.hasError('minlength')) {
        return 'Mínimo de 2 caracteres';
      }
      if (control.hasError('maxlength')) {
        return 'Máximo de 100 caracteres';
      }
    }

    if (controlName === 'year') {
      if (control.hasError('min')) {
        return 'Ano deve ser maior que 1900';
      }
      if (control.hasError('max')) {
        return `Ano não pode ser maior que ${new Date().getFullYear() + 1}`;
      }
    }

    if (controlName === 'mileage') {
      if (control.hasError('min')) {
        return 'Quilometragem não pode ser negativa';
      }
    }

    return '';
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;

    if (this.isEditMode && this.vehicle) {
      this.updateVehicle();
    } else {
      this.createVehicle();
    }
  }

  private createVehicle(): void {
    const request: CreateVehicleRequest = this.form.value;
    
    this.vehicleService.create(request).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Veículo criado com sucesso');
        this.loading = false;
        this.onSuccess.emit();
      },
      error: (error) => {
        const message = error?.error?.error || 'Falha ao criar veículo';
        this.toastService.error('Erro', message);
        this.loading = false;
      }
    });
  }

  private updateVehicle(): void {
    if (!this.vehicle) return;

    const request: UpdateVehicleRequest = {
      model: this.form.value.model,
      year: this.form.value.year,
      mileage: this.form.value.mileage
    };

    this.vehicleService.update(this.vehicle.id, request).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Veículo atualizado com sucesso');
        this.loading = false;
        this.onSuccess.emit();
      },
      error: (error) => {
        const message = error?.error?.error || 'Falha ao atualizar veículo';
        this.toastService.error('Erro', message);
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.onClose.emit();
  }
}
