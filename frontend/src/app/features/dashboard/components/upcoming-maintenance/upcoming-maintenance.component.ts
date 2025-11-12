import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { Vehicle } from '../../../../core/models';
import { StatusBadgeComponent } from '../../../../shared/components/molecules/status-badge/status-badge.component';
import { DateFormatPipe } from '../../../../shared/pipes';

@Component({
  selector: 'app-upcoming-maintenance',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    TableModule,
    StatusBadgeComponent,
    DateFormatPipe
  ],
  templateUrl: './upcoming-maintenance.component.html',
  styleUrls: ['./upcoming-maintenance.component.scss']
})
export class UpcomingMaintenanceComponent {
  @Input() vehicles: Vehicle[] = [];
  @Input() loading = false;
}
