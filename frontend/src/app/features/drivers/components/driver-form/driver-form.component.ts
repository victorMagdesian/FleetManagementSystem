import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { InputMaskModule } from 'primeng/inputmask';
import { DriverService, ToastService } from '../../../../core/services';
import { Driver, CreateDriverRequest, UpdateDriverRequest } from '../../../../core/models';

@Component({
  selector: 'app-driver-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputMaskModule
  ],
  templateUrl: './driver-form.component.html',
  styleUrls: ['./driver-form.component.scss']
})
export class DriverFormComponent implements OnChanges {
  @Input() visible = false;
  @Input() driver: Driver | null = null;
  @Output() onClose = new EventEmitter<void>();
  @Output() onSuccess = new EventEmitter<void>();

  form: FormGroup;
  loading = false;
  isEditMode = false;

  constructor(
    private fb: FormBuilder,
    private driverService: DriverService,
    private toastService: ToastService
  ) {
    this.form = this.createForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['driver'] || changes['visible']) {
      if (this.visible) {
        this.isEditMode = !!this.driver;
        this.resetForm();
      }
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      name: ['', [
        Validators.required,
        Validators.minLength(3),
        Validators.maxLength(100)
      ]],
      licenseNumber: ['', [
        Validators.required,
        Validators.pattern(/^\d{11}$/)
      ]],
      phone: ['', [
        Validators.required,
        Validators.pattern(/^\+55\d{11}$/)
      ]]
    });
  }

  private resetForm(): void {
    if (this.driver) {
      this.form.patchValue({
        name: this.driver.name,
        licenseNumber: this.driver.licenseNumber,
        phone: this.driver.phone
      });
    } else {
      this.form.reset();
    }
  }

  get dialogTitle(): string {
    return this.isEditMode ? 'Editar Condutor' : 'Novo Condutor';
  }

  get nameControl() {
    return this.form.get('name');
  }

  get licenseNumberControl() {
    return this.form.get('licenseNumber');
  }

  get phoneControl() {
    return this.form.get('phone');
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

    if (controlName === 'name') {
      if (control.hasError('minlength')) {
        return 'Mínimo de 3 caracteres';
      }
      if (control.hasError('maxlength')) {
        return 'Máximo de 100 caracteres';
      }
    }

    if (controlName === 'licenseNumber') {
      if (control.hasError('pattern')) {
        return 'CNH deve conter 11 dígitos';
      }
    }

    if (controlName === 'phone') {
      if (control.hasError('pattern')) {
        return 'Formato inválido (ex: +5511999999999)';
      }
    }

    return '';
  }

  onPhoneChange(event: any): void {
    // Remove formatting and keep only digits with +55 prefix
    let value = event.target.value.replace(/\D/g, '');
    if (value.startsWith('55')) {
      value = '+' + value;
    } else if (!value.startsWith('+55')) {
      value = '+55' + value;
    }
    this.form.patchValue({ phone: value }, { emitEvent: false });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;

    if (this.isEditMode && this.driver) {
      this.updateDriver();
    } else {
      this.createDriver();
    }
  }

  private createDriver(): void {
    const request: CreateDriverRequest = this.form.value;
    
    this.driverService.create(request).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Condutor criado com sucesso');
        this.loading = false;
        this.onSuccess.emit();
      },
      error: (error) => {
        const message = error?.error?.error || 'Falha ao criar condutor';
        this.toastService.error('Erro', message);
        this.loading = false;
      }
    });
  }

  private updateDriver(): void {
    if (!this.driver) return;

    const request: UpdateDriverRequest = this.form.value;

    this.driverService.update(this.driver.id, request).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Condutor atualizado com sucesso');
        this.loading = false;
        this.onSuccess.emit();
      },
      error: (error) => {
        const message = error?.error?.error || 'Falha ao atualizar condutor';
        this.toastService.error('Erro', message);
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.onClose.emit();
  }
}
