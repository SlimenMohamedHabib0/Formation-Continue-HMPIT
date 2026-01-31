import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ProfessorDashboardDto } from '../interfaces/iprofessor-dashboard';

@Injectable({
  providedIn: 'root',
})
export class ProfStatsSrvc {
  constructor(private http: HttpClient) {}

  getDashboard(): Observable<ProfessorDashboardDto> {
    return this.http.get<ProfessorDashboardDto>(`${environment.apiUrl}/professor/dashboard`);
  }
}
