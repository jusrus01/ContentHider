import { HttpClient } from '@angular/common/http';
import { Component, Input } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { API_URL } from 'src/app/constants';

@Component({
  selector: 'app-image',
  templateUrl: './image.component.html',
})
export class ImageComponent {
  @Input('containerClass') containerClass!: string;
  @Input('imageClass') imageClass!: string;
  @Input('imageId') imageId: string | null;
  @Input('imageSource') imageSource: string | ArrayBuffer | null;;

  public imageUrl: string | null | SafeUrl;

  constructor(private http: HttpClient, private sanitizer: DomSanitizer) {
    this.imageUrl = null;
    this.imageId = null;
    this.imageSource = null;
  }

  public ngOnInit() {
    this._init();
  }

  public reload(): void {
    this._init();
  }

  private _init(): void {
    if (!this.imageId) {
      return;
    }

    this.http
      .get(`${API_URL}/image/${this.imageId}`, { responseType: 'arraybuffer' })
      .subscribe((response) => {
        const blob = new Blob([response], { type: 'image/jpeg' });
        this.imageUrl = this.sanitizer.bypassSecurityTrustUrl(URL.createObjectURL(blob));
      });
  }
}
