import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { API_URL } from '../constants';

@Injectable({
  providedIn: 'root',
})
export class OrgResource {
  constructor(private httpClient: HttpClient) {}

  public readonly text$: Subject<string> = new Subject<string>();

  setText(text: string) {
    this.text$.next(text);
  }

  getOrgs(): Observable<any> {
    return this.httpClient.get<any>(`${API_URL}/organizations`);
  }

  getOrg(orgId: string): Observable<any> {
    return this.httpClient.get<any>(`${API_URL}/organizations/${orgId}`);
  }

  deleteOrg(orgId: string): Observable<any> {
    return this.httpClient.delete<any>(`${API_URL}/organizations/${orgId}`);
  }

  updateOrg(orgId: string, data: any): Observable<any> {
    return this.httpClient.put<any>(`${API_URL}/organizations/${orgId}`, {
      title: data.title,
      description: data.description,
    });
  }

  createOrg(data: any): Observable<any> {
    return this.httpClient.post<any>(`${API_URL}/organizations/`, {
      title: data.title,
      description: data.description,
    });
  }

  // Non org methods below
  getFormat(orgId: any, formatId: any): Observable<any> {
    return this.httpClient.get<any>(
      `${API_URL}/organizations/${orgId}/formats/${formatId}`
    );
  }

  createFormat(orgId: any, data: any): Observable<any> {
    return this.httpClient.post<any>(
      `${API_URL}/organizations/${orgId}/formats`,
      {
        title: data.title,
        description: data.description,
        formatDefinition: data.formatDefinition,
        type: parseInt(data.type),
      }
    );
  }

  updateFormat(orgId: any, formatId: any, data: any): Observable<any> {
    return this.httpClient.put<any>(
      `${API_URL}/organizations/${orgId}/formats/${formatId}`,
      {
        title: data.title,
        description: data.description,
        formatDefinition: data.formatDefinition,
        type: parseInt(data.type),
      }
    );
  }

  createRule(orgId: string, formatId: string, data: any): Observable<any> {
    return this.httpClient.post<any>(
      `${API_URL}/organizations/${orgId}/formats/${formatId}/rules`,
      {
        title: data.title,
        anonymizedField: data.anonymizedField,
      }
    );
  }

  updateRule(
    orgId: string,
    formatId: string,
    ruleId: string,
    data: any
  ): Observable<any> {
    return this.httpClient.put<any>(
      `${API_URL}/organizations/${orgId}/formats/${formatId}/rules/${ruleId}`,
      {
        title: data.title,
        anonymizedField: data.anonymizedField,
      }
    );
  }

  deleteRule(orgId: string, formatId: string, ruleId: string): Observable<any> {
    return this.httpClient.delete<any>(
      `${API_URL}/organizations/${orgId}/formats/${formatId}/rules/${ruleId}`
    );
  }

  getRule(orgId: string, formatId: string, ruleId: string): Observable<any> {
    return this.httpClient.get<any>(
      `${API_URL}/organizations/${orgId}/formats/${formatId}/rules/${ruleId}`
    );
  }

  deleteFormat(orgId: string, formatId: string): Observable<any> {
    return this.httpClient.delete<any>(
      `${API_URL}/organizations/${orgId}/formats/${formatId}`
    );
  }

  applyRulesForText(orgId: any, formatId: any, text: any): Observable<any> {
    return this.httpClient.post<any>(
      `${API_URL}/organizations/${orgId}/formats/${formatId}/rules/apply`,
      {
        text: text,
      }
    );
  }
}
