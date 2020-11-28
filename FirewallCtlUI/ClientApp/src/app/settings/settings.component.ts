import '../dto';
import { Component } from '@angular/core';
import { ApiService } from '../api.service';

@Component({
  selector: 'app-settings-component',
  templateUrl: './settings.component.html'
})
export class SettingsComponent {
  public Settings: FirewallCtlUI.DTO.Settings;
  public CaptureDevices: FirewallCtlUI.DTO.CaptureDevice[];

  constructor(private api: ApiService) {
    this.api.getSettings().subscribe(i => {
      this.Settings = i;
    });
    this.api.getCaptureDevices().subscribe(i => {
      this.CaptureDevices = i;
    })
  }

  public save() {
    this.api.saveSettings(this.Settings);
  }
}
