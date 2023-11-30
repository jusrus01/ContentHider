import { Component } from '@angular/core';
import { AuthService } from 'src/app/services/auth.service';
import { RoutingService } from 'src/app/services/routing.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
})
export class NavbarComponent {
  constructor(
    public routing: RoutingService,
    private authService: AuthService
  ) {}
  public showMenu = false;

  public toggleNavbar(): void {
    this.showMenu = !this.showMenu;
  }

  public logout(): void {
    // this.authService.logout();
    // this.authService.isLoggedIn$.subscribe((isLoggedIn) => {
    //   if (!isLoggedIn) {
    //     this.routing.goToLogin();
    //   }
    // });
    setTimeout(() => this.authService.logout(), 10);
    this.routing.goToLogin();
  }
}
