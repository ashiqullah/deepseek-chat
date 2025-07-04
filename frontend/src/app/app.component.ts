import { Component, OnInit, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { ChatService } from './chat.service';
import { ChatMessage, ChatRequest, ModelProvider, ProviderInfo } from './models/chat.models';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, AfterViewChecked {
  @ViewChild('chatContainer') private chatContainer!: ElementRef;
  
  title = 'DeepSeek Chat';
  messages: ChatMessage[] = [];
  currentMessage = '';
  conversationId = '';
  isLoading = false;
  isConnected = false;
  
  // Provider selection
  selectedProvider: ModelProvider = ModelProvider.DeepSeekAPI;
  availableProviders: ProviderInfo[] = [];
  ModelProvider = ModelProvider; // Make enum available in template

  constructor(private chatService: ChatService) {
    this.conversationId = this.generateConversationId();
  }

  ngOnInit() {
    this.checkConnection();
    this.loadProviders();
  }

  ngAfterViewChecked() {
    this.scrollToBottom();
  }

  checkConnection() {
    this.chatService.healthCheck().subscribe({
      next: () => {
        this.isConnected = true;
      },
      error: (error) => {
        console.error('Connection failed:', error);
        this.isConnected = false;
      }
    });
  }

  loadProviders() {
    this.chatService.getProviderStatus().subscribe({
      next: (response) => {
        this.availableProviders = response.providers;
        // Set default provider to first available one
        const firstAvailable = this.availableProviders.find(p => p.available);
        if (firstAvailable) {
          this.selectedProvider = firstAvailable.value;
        }
      },
      error: (error) => {
        console.error('Failed to load providers:', error);
        // Fallback to default providers
        this.availableProviders = [
          { name: 'DeepSeek API', value: ModelProvider.DeepSeekAPI, available: true },
          { name: 'Local Model', value: ModelProvider.LocalModel, available: false }
        ];
      }
    });
  }

  onProviderChange(event: Event) {
    const target = event.target as HTMLSelectElement;
    if (target) {
      this.selectedProvider = parseInt(target.value) as ModelProvider;
      // Optionally clear chat when switching providers
      this.clearChat();
    }
  }

  getProviderName(provider: ModelProvider): string {
    const providerInfo = this.availableProviders.find(p => p.value === provider);
    return providerInfo?.name || provider.toString();
  }

  isProviderAvailable(provider: ModelProvider): boolean {
    const providerInfo = this.availableProviders.find(p => p.value === provider);
    return providerInfo?.available || false;
  }

  sendMessage() {
    if (!this.currentMessage.trim() || this.isLoading) {
      return;
    }

    if (!this.isProviderAvailable(this.selectedProvider)) {
      this.addErrorMessage(`Selected provider (${this.getProviderName(this.selectedProvider)}) is not available. Please select a different provider.`);
      return;
    }

    const userMessage: ChatMessage = {
      role: 'user',
      content: this.currentMessage,
      timestamp: new Date()
    };

    this.messages.push(userMessage);
    this.isLoading = true;

    const request: ChatRequest = {
      message: this.currentMessage,
      conversationId: this.conversationId,
      provider: this.selectedProvider
    };

    const messageToSend = this.currentMessage;
    this.currentMessage = '';

    this.chatService.sendMessage(request).subscribe({
      next: (response) => {
        if (response.success) {
          const assistantMessage: ChatMessage = {
            role: 'assistant',
            content: response.response,
            timestamp: new Date()
          };
          this.messages.push(assistantMessage);
          this.conversationId = response.conversationId;
        } else {
          this.addErrorMessage(response.error || 'Unknown error occurred');
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error sending message:', error);
        this.addErrorMessage('Failed to send message. Please try again.');
        this.isLoading = false;
      }
    });
  }

  clearChat() {
    this.chatService.clearChatHistory(this.conversationId).subscribe({
      next: () => {
        this.messages = [];
        this.conversationId = this.generateConversationId();
      },
      error: (error) => {
        console.error('Error clearing chat:', error);
      }
    });
  }

  onKeyPress(event: KeyboardEvent) {
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      this.sendMessage();
    }
  }

  private addErrorMessage(error: string) {
    const errorMessage: ChatMessage = {
      role: 'assistant',
      content: `Error: ${error}`,
      timestamp: new Date()
    };
    this.messages.push(errorMessage);
  }

  private generateConversationId(): string {
    return 'conv_' + Math.random().toString(36).substr(2, 9);
  }

  private scrollToBottom(): void {
    try {
      this.chatContainer.nativeElement.scrollTop = this.chatContainer.nativeElement.scrollHeight;
    } catch(err) {}
  }
} 