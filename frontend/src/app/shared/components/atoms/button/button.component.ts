import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { TooltipModule } from 'primeng/tooltip';

@Component({
  selector: 'app-button',
  standalone: true,
  imports: [CommonModule, ButtonModule, TooltipModule],
  templateUrl: './button.component.html',
  styleUrls: ['./button.component.scss']
})
export class ButtonComponent {
  @Input() label?: string;
  @Input() icon?: string;
  @Input() severity: 'primary' | 'secondary' | 'success' | 'info' | 'warning' | 'danger' | 'help' | 'contrast' = 'primary';
  @Input() disabled = false;
  @Input() loading = false;
  @Input() outlined = false;
  @Input() text = false;
  @Input() raised = false;
  @Input() rounded = false;
  @Input() size: 'small' | 'large' | undefined = undefined;
  @Input() tooltip?: string;
  @Input() tooltipPosition: 'top' | 'bottom' | 'left' | 'right' = 'top';
  @Input() type: 'button' | 'submit' | 'reset' = 'button';
  @Input() ariaLabel?: string;
  
  @Output() clicked = new EventEmitter<void>();

  handleClick(): void {
    if (!this.disabled && !this.loading) {
      this.clicked.emit();
    }
  }
}
