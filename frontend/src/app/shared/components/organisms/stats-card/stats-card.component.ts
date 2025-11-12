import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { IconComponent } from '../../atoms/icon/icon.component';

@Component({
  selector: 'app-stats-card',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    IconComponent
  ],
  templateUrl: './stats-card.component.html',
  styleUrls: ['./stats-card.component.scss']
})
export class StatsCardComponent {
  @Input() icon = '';
  @Input() value: number | string = 0;
  @Input() label = '';
  @Input() color: 'primary' | 'success' | 'warning' | 'danger' | 'info' = 'primary';
}
