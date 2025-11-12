import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { Trip, Vehicle, Driver } from '../../../../core/models';
import { VehicleService, DriverService } from '../../../../core/services';
import { DateFormatPipe } from '../../../../shared/pipes';

interface TripWithDetails extends Trip {
  vehiclePlate?: string;
  vehicleModel?: string;
  driverName?: string;
}

@Component({
  selector: 'app-active-trips',
  standalone: true,
  imports: [
    CommonModule,
    CardModule,
    TableModule,
    DateFormatPipe
  ],
  templateUrl: './active-trips.component.html',
  styleUrls: ['./active-trips.component.scss']
})
export class ActiveTripsComponent implements OnChanges {
  @Input() trips: Trip[] = [];
  @Input() loading = false;
  
  tripsWithDetails: TripWithDetails[] = [];
  loadingDetails = false;

  constructor(
    private vehicleService: VehicleService,
    private driverService: DriverService
  ) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['trips'] && this.trips.length > 0) {
      this.loadTripDetails();
    } else if (changes['trips'] && this.trips.length === 0) {
      this.tripsWithDetails = [];
    }
  }

  private loadTripDetails(): void {
    this.loadingDetails = true;
    
    const tripDetailsObservables = this.trips.map(trip => 
      forkJoin({
        vehicle: this.vehicleService.getById(trip.vehicleId).pipe(
          catchError(() => of(null as Vehicle | null))
        ),
        driver: this.driverService.getById(trip.driverId).pipe(
          catchError(() => of(null as Driver | null))
        )
      }).pipe(
        map(({ vehicle, driver }) => ({
          ...trip,
          vehiclePlate: vehicle?.plate || 'N/A',
          vehicleModel: vehicle?.model || 'N/A',
          driverName: driver?.name || 'N/A'
        }))
      )
    );

    forkJoin(tripDetailsObservables).subscribe({
      next: (details) => {
        this.tripsWithDetails = details;
        this.loadingDetails = false;
      },
      error: () => {
        this.tripsWithDetails = this.trips.map(trip => ({
          ...trip,
          vehiclePlate: 'N/A',
          vehicleModel: 'N/A',
          driverName: 'N/A'
        }));
        this.loadingDetails = false;
      }
    });
  }
}
