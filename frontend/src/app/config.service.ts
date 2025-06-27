import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {
  private _apiUrl: string;

  constructor() {
    this._apiUrl = this.determineApiUrl();
  }

  get apiUrl(): string {
    return this._apiUrl;
  }

  private determineApiUrl(): string {
    const host = window.location.hostname;
    const port = window.location.port;
    const protocol = window.location.protocol;
    
    console.log(`🌐 Frontend running on: ${protocol}//${host}:${port}`);
    
    // For local development
    if (host === 'localhost' || host === '127.0.0.1') {
      const apiUrl = 'http://localhost:5000';
      console.log(`🔧 Using local development API URL: ${apiUrl}`);
      return apiUrl;
    }
    
    // For deployment on any IP (including 192.168.0.43) - use same host but port 5000
    const apiUrl = `http://${host}:5000`;
    console.log(`🚀 Using deployment API URL: ${apiUrl}`);
    console.log(`📋 Expected setup:
    - Frontend: http://${host}:4200
    - Backend:  http://${host}:5000`);
    
    return apiUrl;
  }
} 