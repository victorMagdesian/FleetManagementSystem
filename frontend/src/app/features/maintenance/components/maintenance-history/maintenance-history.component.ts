import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DialogModule } from 'primeng/dialog';
import { TableModule } from 'primeng/table';
import { MaintenanceService, ToastService } from '../../../../core/services';
import { Maintenance } from '../../../../core/models';
import { DateFormatPipe, CurrencyFormatPipe } from '../../../../shared/pipes';

@Component({
  selector: 'app-maintenance-history',
  standalone: true,
  imports: [
    CommonModule,
    DialogModule,
    TableModule,
    DateFormatPipe,
    CurrencyFormatPipe
  ],
  templateUrl: './maintenance-history.component.html',
  styleUrls: ['./maintenance-history.component.scss']
})
export class MaintenanceHistoryComponent implements OnChanges {
  @Input() visible = false;
  @Input() vehicleId: string | null = null;
  @Output() onClose = new EventEmitter<void>();

  maintenanceHistory: Maintenance[] = [];
  loading = false;

  constructor(
    private maintenanceService: MaintenanceService,
    private toastService: ToastService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && this.visible && this.vehicleId) {
      this.loadHistory();
    }
  }

  private loadHistory(): void {
    if (!this.vehicleId) return;

    this.loading = true;
    this.maintenanceService.getByVehicle(this.vehicleId).subscribe({
      next: (history) => {
        this.maintenanceHistory = history.sort((a, b) => 
          new Date(b.date).getTime() - new Date(a.date).getTime()
        );
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao carregar histórico de manutenções');
        this.loading = false;
      }
    });
  }

  getTotalCost(): number {
    return this.maintenanceHistory.reduce((sum, m) => sum + m.cost, 0);
  }

  onDialogHide(): void {
    this.onClose.emit();
  }
}
