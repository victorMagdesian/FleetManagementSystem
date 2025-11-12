import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { InputNumberModule } from 'primeng/inputnumber';
import { TripService, ToastService } from '../../../../core/services';
import { Trip, EndTripRequest } from '../../../../core/models';

@Component({
  selector: 'app-end-trip-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    DialogModule,
    ButtonModule,
    InputNumberModule
  ],
  templateUrl: './end-trip-form.component.html',
  styleUrls: ['./end-trip-form.component.scss']
})
export class EndTripFormComponent implements OnChanges {
  @Input() visible = false;
  @Input() trip: Trip | null = null;
  @Output() onClose = new EventEmitter<void>();
  @Output() onSuccess = new EventEmitter<void>();

  form: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private tripService: TripService,
    private toastService: ToastService
  ) {
    this.form = this.createForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && this.visible) {
      this.resetForm();
    }
  }

  private createForm(): FormGroup {
    return this.fb.group({
      distance: [null, [
        Validators.required,
        Validators.min(1)
      ]]
    });
  }

  private resetForm(): void {
    this.form.reset();
  }

  get distanceControl() {
    return this.form.get('distance');
  }

  getErrorMessage(controlName: string): string {
    const control = this.form.get(controlName);
    
    if (!control || !control.errors || !(control.dirty || control.touched)) {
      return '';
    }

    if (control.hasError('required')) {
      return 'Campo obrigatório';
    }

    if (controlName === 'distance') {
      if (control.hasError('min')) {
        return 'Distância deve ser maior que zero';
      }
    }

    return '';
  }

  onSubmit(): void {
    if (this.form.invalid || !this.trip) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    const request: EndTripRequest = {
      distance: this.form.value.distance
    };
    
    this.tripService.end(this.trip.id, request).subscribe({
      next: () => {
        this.toastService.success('Sucesso', 'Viagem finalizada com sucesso');
        this.loading = false;
        this.onSuccess.emit();
      },
      error: (error) => {
        const message = error?.error?.error || 'Falha ao finalizar viagem';
        this.toastService.error('Erro', message);
        this.loading = false;
      }
    });
  }

  onCancel(): void {
    this.onClose.emit();
  }
}
