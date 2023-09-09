import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { take } from 'rxjs';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule, TimeagoModule]
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?: NgForm;
  @Input() username?: string;
  messageContent = '';
  loading = false;
  currentUser: User | null = null;

  constructor(public messageService: MessageService, public accountService: AccountService) {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => this.currentUser = user
    })
  }

  ngOnInit(): void { }

  sendMessage() {
    if (!this.username) return;
    this.loading = true;
    this.messageService.sendMessage(this.username, this.messageContent).then(() => {      // instead of 'subscribe', we use 'then' here (because it is a promise)
      this.messageForm?.reset();
    }).finally(() => this.loading = false);
  }

}
