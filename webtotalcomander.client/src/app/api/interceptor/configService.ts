import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private serverUrl = 'https://localhost:7142/api/';

  constructor() { }

  getServerUrl(): string {
    return this.serverUrl;
  }
}
