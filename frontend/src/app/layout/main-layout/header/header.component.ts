import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, ButtonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent {
  @Input() title = 'Dashboard';
  @Output() menuToggle = new EventEmitter<void>();

  onMenuToggle(): void {
    this.menuToggle.emit();
  }
}
