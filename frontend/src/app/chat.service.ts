import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChatRequest, ChatResponse, ChatMessage } from './models/chat.models';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

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
    return this.http.get(`${this.apiUrl}/api/chat/health`);
  }
} 