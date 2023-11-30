import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from './guard/auth.guard';
import { LoginComponent } from './pages/login/login.component';
import { OrganizationComponent } from './pages/organizations/organization.component';
import { OrganizationItemComponent } from './pages/organizations-item/organization-item.component';
import { HomeComponent } from './pages/home/home.component';
import { OrganizationConfiguratiomComponent } from './pages/organizations-item/configuration/organization-configuration.component';
import { AdminGuard } from './guard/admin.guard';
import { FormatItemComponent } from './pages/formats/format-item.component';
import { FormatConfigurationComponent } from './pages/formats/format-configuration/format-configuration.component';
import { RuleConfigurationComponent } from './pages/rules/rule-configuration/rule-configuration.component';

export const ROUTES: Routes = [
  {
    path: 'organizations/:orgId/formats/:formatId/rules/:ruleId/edit',
    data: { title: 'organization' },
    canActivate: [AdminGuard],
    component: RuleConfigurationComponent,
  },
  {
    path: 'organizations/:orgId/formats/:formatId/edit',
    data: { title: 'organization' },
    canActivate: [AdminGuard],
    component: FormatConfigurationComponent,
  },
  {
    path: 'organizations/:orgId/formats/:formatId',
    data: { title: 'organization' },
    canActivate: [AdminGuard],
    component: FormatItemComponent,
  },
  {
    path: 'organizations/create',
    component: OrganizationConfiguratiomComponent,
    data: { title: 'organization' },
    canActivate: [AdminGuard],
  },
  {
    path: 'organizations/:id/edit',
    component: OrganizationConfiguratiomComponent,
    data: { title: 'organization' },
    canActivate: [AdminGuard],
  },
  {
    path: 'organizations/:id',
    component: OrganizationItemComponent,
    data: { title: 'organization' },
    canActivate: [AuthGuard],
  },
  {
    path: 'organizations',
    component: OrganizationComponent,
    canActivate: [AuthGuard],
  },
  { path: 'login', component: LoginComponent },
  // TODO: rename
  { path: 'not-found', component: HomeComponent },
  {
    path: '**',
    redirectTo: '',
    component: OrganizationComponent,
    canActivate: [AuthGuard],
  },
];

@NgModule({
  imports: [RouterModule.forRoot(ROUTES)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
