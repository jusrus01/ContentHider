import { Directive, TemplateRef, ViewContainerRef } from '@angular/core';
import { ADMINISTRATOR_ROLE } from 'src/app/constants';
import { AuthService } from 'src/app/services/auth.service';

@Directive({
  selector: '[showForAdminUser]',
})
export class ShowForAdminUserDirective {
  constructor(
    private authService: AuthService,
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {}

  ngOnInit() {
    this.resolveView(this.authService.isLoggedIn(), this.authService.hasRole(ADMINISTRATOR_ROLE));
    this.authService.isLoggedIn$.subscribe((isLoggedIn: boolean) => {
      this.resolveView(isLoggedIn, this.authService.hasRole(ADMINISTRATOR_ROLE));
    });
  }

  private resolveView(isLoggedIn: boolean, isAdmin: boolean) {
    if (isLoggedIn && isAdmin) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }
}
