import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StatsCardComponent } from '../../../../shared/components/organisms/stats-card/stats-card.component';

@Component({
  selector: 'app-fleet-summary',
  standalone: true,
  imports: [
    CommonModule,
    StatsCardComponent
  ],
  templateUrl: './fleet-summary.component.html',
  styleUrls: ['./fleet-summary.component.scss']
})
export class FleetSummaryComponent {
  @Input() availableVehicles = 0;
  @Input() inUseVehicles = 0;
  @Input() inMaintenanceVehicles = 0;
  @Input() activeTrips = 0;
  @Input() loading = false;
}
