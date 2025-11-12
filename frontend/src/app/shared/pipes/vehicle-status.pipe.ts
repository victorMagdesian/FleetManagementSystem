import { Pipe, PipeTransform } from '@angular/core';
import { VehicleStatus } from '../../core/models/vehicle.model';

@Pipe({
  name: 'vehicleStatus',
  standalone: true
})
export class VehicleStatusPipe implements PipeTransform {
  private readonly statusMap: Record<VehicleStatus, string> = {
    [VehicleStatus.Available]: 'Disponível',
    [VehicleStatus.InUse]: 'Em Uso',
    [VehicleStatus.InMaintenance]: 'Em Manutenção'
  };

  transform(value: VehicleStatus | null | undefined): string {
    if (!value) return '';
    
    return this.statusMap[value] || value;
  }
}
