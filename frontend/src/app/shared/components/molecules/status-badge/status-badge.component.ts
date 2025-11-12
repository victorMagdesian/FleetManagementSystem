import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { BadgeComponent } from '../../atoms/badge/badge.component';
import { VehicleStatus } from '../../../../core/models/vehicle.model';

type StatusType = VehicleStatus | 'active' | 'inactive';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule, BadgeComponent],
  templateUrl: './status-badge.component.html',
  styleUrls: ['./status-badge.component.scss']
})
export class StatusBadgeComponent {
  @Input() status!: StatusType;

  get statusLabel(): string {
    const labels: Record<string, string> = {
      [VehicleStatus.Available]: 'Disponível',
      [VehicleStatus.InUse]: 'Em Uso',
      [VehicleStatus.InMaintenance]: 'Em Manutenção',
      'active': 'Ativo',
      'inactive': 'Inativo'
    };
    return labels[this.status] || this.status;
  }

  get statusSeverity(): 'success' | 'info' | 'warning' | 'danger' | 'secondary' {
    const severities: Record<string, 'success' | 'info' | 'warning' | 'danger' | 'secondary'> = {
      [VehicleStatus.Available]: 'success',
      [VehicleStatus.InUse]: 'info',
      [VehicleStatus.InMaintenance]: 'warning',
      'active': 'success',
      'inactive': 'secondary'
    };
    return severities[this.status] || 'info';
  }

  get statusIcon(): string | undefined {
    const icons: Record<string, string> = {
      [VehicleStatus.Available]: 'pi pi-check-circle',
      [VehicleStatus.InUse]: 'pi pi-car',
      [VehicleStatus.InMaintenance]: 'pi pi-wrench',
      'active': 'pi pi-check',
      'inactive': 'pi pi-times'
    };
    return icons[this.status];
  }
}
