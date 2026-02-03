import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, NavigationEnd } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { filter, Subscription } from 'rxjs';
import { PublicSrvc } from '../public-srvc';

type CourseDetails = {
  id: number;
  titre: string;
  nomFichierPdf?: string | null;

  videoFileName?: string | null;
  videoPath?: string | null;
  videoMimeType?: string | null;
};

@Component({
  selector: 'app-lecture-cours',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lecture-cours.html',
  styleUrl: './lecture-cours.css',
})
export class LectureCours implements OnInit, OnDestroy {
  loading = true;
  saving = false;

  error: string | null = null;
  success: string | null = null;

  courseId: number | null = null;
  course: CourseDetails | null = null;

  pdfUrl: SafeResourceUrl | null = null;
  private pdfObjectUrl: string | null = null;

  videoUrl: SafeResourceUrl | null = null;
  private videoObjectUrl: string | null = null;
  private navSub?: Subscription;

  completed = false;

  hasVideo = false;
  hasPdf = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private srvc: PublicSrvc,
    private sanitizer: DomSanitizer,
    
  ) {}

  ngOnInit(): void {
    const n = Number(this.route.snapshot.paramMap.get('courseId'));
    this.courseId = Number.isFinite(n) ? n : null;

    if (!this.courseId) {
      this.loading = false;
      this.error = 'Identifiant du cours invalide.';
      return;
    }

    this.loadAll();
    this.navSub = this.router.events
  .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
  .subscribe(() => {
    if (this.courseId) {
      this.loadProgress();
    }
  });

  }

  ngOnDestroy(): void {
    if (this.pdfObjectUrl) URL.revokeObjectURL(this.pdfObjectUrl);
    if (this.videoObjectUrl) URL.revokeObjectURL(this.videoObjectUrl);
    this.navSub?.unsubscribe();

  }

  back(): void {
    this.router.navigate(['/public/mes-inscriptions']);
  }

  resetMsgs(): void {
    this.error = null;
    this.success = null;
  }

  private loadAll(): void {
    this.loading = true;
    this.error = null;

    this.loadProgress();

    this.srvc.getCourseById(this.courseId!).subscribe({
      next: (c: CourseDetails) => {
        this.course = c;

        this.hasPdf = !!c?.nomFichierPdf;
        this.hasVideo = !!(c?.videoFileName || c?.videoPath);

        if (this.hasVideo) this.loadVideoBlob();
        else this.videoUrl = null;

        if (this.hasPdf) this.loadPdf();
        else this.pdfUrl = null;

        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.error = 'Impossible de charger le cours.';
      },
    });
  }

  private loadVideoBlob(): void {
    if (!this.courseId) return;

    this.srvc.getCourseVideoBlob(this.courseId).subscribe({
      next: (blob: Blob) => {
        if (this.videoObjectUrl) URL.revokeObjectURL(this.videoObjectUrl);

        this.videoObjectUrl = URL.createObjectURL(blob);
        this.videoUrl = this.sanitizer.bypassSecurityTrustResourceUrl(this.videoObjectUrl);
      },
      error: () => {
        this.videoUrl = null;
      },
    });
  }

  private loadProgress(): void {
    if (!this.courseId) return;
  
    this.srvc.getProgress(this.courseId).subscribe({
      next: (p: any) => {
        this.completed = !!p?.dateCompletion; 
      },
      error: () => {
        this.completed = false;
      },
    });
  }
  

  private loadPdf(): void {
    this.srvc.getCoursePdfBlob(this.courseId!).subscribe({
      next: (blob: Blob) => {
        if (this.pdfObjectUrl) URL.revokeObjectURL(this.pdfObjectUrl);

        this.pdfObjectUrl = URL.createObjectURL(blob);
        this.pdfUrl = this.sanitizer.bypassSecurityTrustResourceUrl(
          this.pdfObjectUrl + '#view=FitH&zoom=page-width'
        );
      },
      error: () => {
        this.pdfUrl = null;
      },
    });
  }
  finish(): void {
    if (!this.courseId) return;
    if (this.completed) return;

    this.resetMsgs();
    this.saving = true;

    this.srvc.updateProgress(this.courseId, 1, 1).subscribe({
      next: () => {
        this.saving = false;
        this.completed = true;
        this.success = 'Cours marqué comme terminé.';
      },
      error: (e: any) => {
        this.saving = false;
        const msg = typeof e?.error === 'string' ? e.error : '';
        this.error = msg ? msg : 'Action impossible.';
      },
    });
  }

  goTest(): void {
    if (!this.courseId) return;
    this.router.navigate(['/public/passer-test', this.courseId]);
  }
}
