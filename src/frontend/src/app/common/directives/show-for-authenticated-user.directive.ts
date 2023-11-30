import { Directive, TemplateRef, ViewContainerRef } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';

@Directive({
  selector: '[showForAuthenticatedUser]',
})
export class ShowForAuthenticatedUserDirective {
  constructor(
    private authService: AuthService,
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {}

  ngOnInit() {
    this.resolveView(this.authService.isLoggedIn());
    this.authService.isLoggedIn$.subscribe((isLoggedIn: boolean) => {
      this.resolveView(isLoggedIn);
    })
  }

  private resolveView(isLoggedIn: boolean) {
    if (isLoggedIn) {
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }
}
