import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { ButtonComponent } from '../../atoms/button/button.component';

@Component({
  selector: 'app-form-dialog',
  standalone: true,
  imports: [
    CommonModule,
    DialogModule,
    ButtonComponent
  ],
  templateUrl: './form-dialog.component.html',
  styleUrls: ['./form-dialog.component.scss']
})
export class FormDialogComponent {
  @Input() title = '';
  @Input() visible = false;
  @Input() loading = false;
  @Input() isValid = true;
  @Input() width = '500px';
  @Input() saveButtonLabel = 'Salvar';
  @Input() cancelButtonLabel = 'Cancelar';
  
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<void>();
  @Output() cancel = new EventEmitter<void>();
  
  onHide(): void {
    this.visible = false;
    this.visibleChange.emit(false);
  }
  
  onCancel(): void {
    this.cancel.emit();
    this.onHide();
  }
  
  onSave(): void {
    if (this.isValid && !this.loading) {
      this.save.emit();
    }
  }
}
