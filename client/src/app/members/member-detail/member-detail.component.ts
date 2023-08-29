import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { TimeagoModule } from 'ngx-timeago';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';

@Component({
  standalone: true,
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [GalleryModule, TabsModule, CommonModule, TimeagoModule, MemberMessagesComponent]
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static: true}) memberTabs?: TabsetComponent;      // this is dynamic by default; if we set it to static, then it means that "memberTabs" should be constructed immideately. However, for this to work, we need to remove any conditionals inside our template. Tldr: static is required for query params to work OUTSIDE this component
  member: Member = {} as Member;      // this initializes our member with an empty object , but should later be populated from our route by the route resolver ! ! !
  activeTab?: TabDirective;
  messages: Message[] = [];
  user?: User;
  images: GalleryItem[] = [];

  constructor(private accountService: AccountService, private route: ActivatedRoute,
    private messageService: MessageService, public presenceService: PresenceService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next: user => {
          if (user) this.user = user;
        }
      })
    }

  ngOnInit(): void {
    this.route.data.subscribe({
      next: data => this.member = data['member']        // 'member' param should be the same as the corresponding property in app-routing-module
    })

    this.route.queryParams.subscribe({      // since the component is loaded before the template, at this stage we do not have access to the template (and with this, an access to the template tabs)
      next: params => {
        params['tab'] && this.selectTab(params['tab'])
      }
    })

    this.getImages();

    // this.galleryImages = this.getImages();         // Now it is commented because this function would run and finish BEFORE the above-wirtten "this.loadMember()" would return the requested data, so that's why we moved it direcrly to the loadMember() function to ensure we retrieve images strictly after we requested all other requested data
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  onTabActivated(data: TabDirective) {
    this.activeTab = data;
    if (this.activeTab.heading === 'Messages' && this.user) {
      // OLD:
      // this.loadMessages();
      this.messageService.createHubConnection(this.user, this.member.userName);
    } else {
      this.messageService.stopHubConnection();      // just this line is not enough, we also need to do this when this component is destroyed (which we eventually did in the ngOnDesrtoy() method)
    }
  }

  loadMessages() {
    if (this.member) {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next: messages => this.messages = messages
      })
    }
  }

  getImages() {
    if (!this.member) return;
    for (const photo of this.member.photos) {
      this.images.push(new ImageItem({src: photo.url, thumb: photo.url}))
    }
  }

  selectTab(heading: string) {
    if (this.memberTabs) {
      this.memberTabs.tabs.find(x => x.heading === heading)!.active = true;     // there should have been a "?" instead of "!", but because TS argues about ?, we put ! (we tell TS that we know better in this circumstance)
    }
  }

}
