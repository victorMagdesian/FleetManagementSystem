import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { forkJoin } from 'rxjs';
import { VehicleService, TripService, MaintenanceService } from '../../../../core/services';
import { Vehicle, Trip, VehicleStatus } from '../../../../core/models';
import { FleetSummaryComponent } from '../../components/fleet-summary/fleet-summary.component';
import { UpcomingMaintenanceComponent } from '../../components/upcoming-maintenance/upcoming-maintenance.component';
import { ActiveTripsComponent } from '../../components/active-trips/active-trips.component';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [
    CommonModule,
    FleetSummaryComponent,
    UpcomingMaintenanceComponent,
    ActiveTripsComponent
  ],
  templateUrl: './dashboard-page.component.html',
  styleUrls: ['./dashboard-page.component.scss']
})
export class DashboardPageComponent implements OnInit {
  availableVehicles = 0;
  inUseVehicles = 0;
  inMaintenanceVehicles = 0;
  activeTripsCount = 0;
  upcomingMaintenance: Vehicle[] = [];
  activeTrips: Trip[] = [];
  loading = true;

  constructor(
    private vehicleService: VehicleService,
    private tripService: TripService,
    private maintenanceService: MaintenanceService
  ) {}

  ngOnInit(): void {
    this.loadDashboardData();
  }

  private loadDashboardData(): void {
    this.loading = true;
    
    forkJoin({
      vehicles: this.vehicleService.getAll(),
      trips: this.tripService.getActive(),
      upcoming: this.maintenanceService.getUpcoming(7)
    }).subscribe({
      next: (data) => {
        this.calculateStats(data.vehicles);
        this.activeTrips = data.trips;
        this.activeTripsCount = data.trips.length;
        this.upcomingMaintenance = data.upcoming;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  private calculateStats(vehicles: Vehicle[]): void {
    this.availableVehicles = vehicles.filter(v => v.status === VehicleStatus.Available).length;
    this.inUseVehicles = vehicles.filter(v => v.status === VehicleStatus.InUse).length;
    this.inMaintenanceVehicles = vehicles.filter(v => v.status === VehicleStatus.InMaintenance).length;
  }
}
