export enum ModelProvider {
  DeepSeekAPI = 0,
  LocalModel = 1
}

export interface ChatRequest {
  message: string;
  conversationId?: string;
  provider: ModelProvider;
}

export interface ChatResponse {
  response: string;
  conversationId: string;
  success: boolean;
  error?: string;
  provider: ModelProvider;
}

export interface ChatMessage {
  role: string; // 'user' or 'assistant'
  content: string;
  timestamp: Date;
}

export interface ProviderInfo {
  name: string;
  value: ModelProvider;
  available: boolean;
} 