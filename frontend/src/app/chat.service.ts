import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChatRequest, ChatResponse, ChatMessage } from './models/chat.models';
import { ConfigService } from './config.service';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private apiUrl: string;

  constructor(private http: HttpClient, private config: ConfigService) {
    this.apiUrl = this.config.apiUrl;
    console.log(`ChatService initialized with API URL: ${this.apiUrl}`);
  }

  sendMessage(request: ChatRequest): Observable<ChatResponse> {
    return this.http.post<ChatResponse>(`${this.apiUrl}/api/chat/send`, request);
  }

  getChatHistory(conversationId: string): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.apiUrl}/api/chat/history/${conversationId}`);
  }

  clearChatHistory(conversationId: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/api/chat/history/${conversationId}`);
  }

  healthCheck(): Observable<any> {
    console.log(`Health check URL: ${this.apiUrl}/api/chat/health`);
    return this.http.get(`${this.apiUrl}/api/chat/health`);
  }
} 