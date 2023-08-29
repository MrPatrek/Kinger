import { ResolveFn } from "@angular/router";
import { Member } from "../_models/member";
import { inject } from "@angular/core";
import { MembersService } from "../_services/members.service";

export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {
  const memberService = inject(MembersService);

  return memberService.getMember(route.paramMap.get('username')!)       // with "!" we tell TS that we are sure that we will have a username when this method is called
}
