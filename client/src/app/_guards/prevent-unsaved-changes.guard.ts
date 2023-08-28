import { CanDeactivateFn } from "@angular/router";
import { MemberEditComponent } from "../members/member-edit/member-edit.component";
import { inject } from "@angular/core";
import { ConfirmService } from "../_services/confirm.service";

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  const confirmService = inject(ConfirmService);

  if (component.editForm?.dirty) {
    // Old, ordinary "modal":
    // return confirm('Are you sure you want to continue? Any unsaved changes will be lost')

    // New, nice-looking Bootstrap modal:
    return confirmService.confirm();
  }

  return true;        // just to satisfy TypeScript
};
