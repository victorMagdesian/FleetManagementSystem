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
        Validators.pattern(/^\+55\s?\(?\d{2}\)?\s?\d{4,5}-?\d{4}$/)
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
        return 'Formato inválido (ex: +55 (11) 99999-9999)';
      }
    }

    return '';
  }

  onPhoneChange(event: any): void {
    // This method is called when the mask is complete
    // The normalization happens in onSubmit, so we don't need to do anything here
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

  private normalizePhone(phone: string): string {
    // Remove all non-digit characters except +
    const cleaned = phone.replace(/[^\d+]/g, '');
    // Ensure it starts with +55
    if (cleaned.startsWith('+55')) {
      return cleaned;
    } else if (cleaned.startsWith('55')) {
      return '+' + cleaned;
    } else {
      return '+55' + cleaned;
    }
  }

  private createDriver(): void {
    const formValue = this.form.value;
    const request: CreateDriverRequest = {
      ...formValue,
      phone: this.normalizePhone(formValue.phone)
    };
    
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

    const formValue = this.form.value;
    const request: UpdateDriverRequest = {
      ...formValue,
      phone: this.normalizePhone(formValue.phone)
    };

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
