import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Trip, StartTripRequest, EndTripRequest } from '../models';

@Injectable({
  providedIn: 'root'
})
export class TripService {
  private readonly endpoint = '/api/trips';

  constructor(private api: ApiService) {}

  getAll(): Observable<Trip[]> {
    return this.api.get<Trip[]>(this.endpoint);
  }

  getById(id: string): Observable<Trip> {
    return this.api.get<Trip>(`${this.endpoint}/${id}`);
  }

  getActive(): Observable<Trip[]> {
    return this.api.get<Trip[]>(`${this.endpoint}/active`);
  }

  start(request: StartTripRequest): Observable<Trip> {
    return this.api.post<Trip>(`${this.endpoint}/start`, request);
  }

  end(id: string, request: EndTripRequest): Observable<Trip> {
    return this.api.post<Trip>(`${this.endpoint}/end/${id}`, request);
  }
}
