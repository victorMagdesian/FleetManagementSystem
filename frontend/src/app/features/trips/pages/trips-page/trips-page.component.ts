import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { TabViewModule } from 'primeng/tabview';
import { TagModule } from 'primeng/tag';
import { TripService, ToastService } from '../../../../core/services';
import { Trip } from '../../../../core/models';
import { StartTripFormComponent } from '../../components/start-trip-form/start-trip-form.component';
import { EndTripFormComponent } from '../../components/end-trip-form/end-trip-form.component';
import { DateFormatPipe } from '../../../../shared/pipes';

@Component({
  selector: 'app-trips-page',
  standalone: true,
  imports: [
    CommonModule,
    TableModule,
    ButtonModule,
    TabViewModule,
    TagModule,
    StartTripFormComponent,
    EndTripFormComponent,
    DateFormatPipe
  ],
  templateUrl: './trips-page.component.html',
  styleUrls: ['./trips-page.component.scss']
})
export class TripsPageComponent implements OnInit {
  allTrips: Trip[] = [];
  activeTrips: Trip[] = [];
  loading = false;
  showStartTripDialog = false;
  showEndTripDialog = false;
  selectedTrip: Trip | null = null;

  constructor(
    private tripService: TripService,
    private toastService: ToastService
  ) {}

  ngOnInit(): void {
    this.loadTrips();
  }

  loadTrips(): void {
    this.loading = true;
    this.tripService.getAll().subscribe({
      next: (trips) => {
        this.allTrips = trips;
        this.activeTrips = trips.filter(trip => !trip.endDate);
        this.loading = false;
      },
      error: () => {
        this.toastService.error('Erro', 'Falha ao carregar viagens');
        this.loading = false;
      }
    });
  }

  openStartTripDialog(): void {
    this.showStartTripDialog = true;
  }

  openEndTripDialog(trip: Trip): void {
    this.selectedTrip = trip;
    this.showEndTripDialog = true;
  }

  onStartTripClose(): void {
    this.showStartTripDialog = false;
  }

  onStartTripSuccess(): void {
    this.showStartTripDialog = false;
    this.loadTrips();
  }

  onEndTripClose(): void {
    this.showEndTripDialog = false;
    this.selectedTrip = null;
  }

  onEndTripSuccess(): void {
    this.showEndTripDialog = false;
    this.selectedTrip = null;
    this.loadTrips();
  }

  getTripStatus(trip: Trip): string {
    return trip.endDate ? 'Finalizada' : 'Ativa';
  }

  getTripStatusSeverity(trip: Trip): 'success' | 'info' {
    return trip.endDate ? 'success' : 'info';
  }
}
