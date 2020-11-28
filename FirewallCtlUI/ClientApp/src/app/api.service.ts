import './dto';
import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class ApiService {
  constructor(private http: HttpClient) { }

  public getSettings(): Observable<FirewallCtlUI.DTO.Settings> {
    return this.http.post<FirewallCtlUI.DTO.Settings>('api/GetSettings', {});
  }

  public saveSettings(data: FirewallCtlUI.DTO.Settings): void {
    this.http.post('api/SaveSettings', data).toPromise();
  }

  public getCaptureDevices(): Observable<FirewallCtlUI.DTO.CaptureDevice[]> {
    return this.http.post<FirewallCtlUI.DTO.CaptureDevice[]>('api/GetDevices', {});
  }
}
