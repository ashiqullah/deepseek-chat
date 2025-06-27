export interface ChatRequest {
  message: string;
  conversationId?: string;
}

export interface ChatResponse {
  response: string;
  conversationId: string;
  success: boolean;
  error?: string;
}

export interface ChatMessage {
  role: string; // 'user' or 'assistant'
  content: string;
  timestamp: Date;
} 