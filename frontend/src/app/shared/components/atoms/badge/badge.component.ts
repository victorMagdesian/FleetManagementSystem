import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TagModule } from 'primeng/tag';

@Component({
  selector: 'app-badge',
  standalone: true,
  imports: [CommonModule, TagModule],
  templateUrl: './badge.component.html',
  styleUrls: ['./badge.component.scss']
})
export class BadgeComponent {
  @Input() label!: string;
  @Input() severity: 'success' | 'info' | 'warning' | 'danger' | 'secondary' | 'contrast' = 'info';
  @Input() icon?: string;
  @Input() rounded = false;
}
