import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { PublicSrvc } from '../public-srvc';

type AlertType = 'success' | 'danger' | 'info';

interface UserQcmDto {
  courseId: number;
  totalPoints: number;
  questions: UserQcmQuestionDto[];
}

interface UserQcmQuestionDto {
  questionId: number;
  enonce: string;
  points: number;
  choix: UserQcmChoiceDto[];
}

interface UserQcmChoiceDto {
  choixId: number;
  libelle: string;
}

interface UserQcmSubmitResultDto {
  tentativeId: number;
  noteSur20: number;
  statutTentative: 'REUSSI' | 'ECHOUE';
}

@Component({
  selector: 'app-passer-test',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './passer-test.html',
  styleUrl: './passer-test.css',
})
export class PasserTest implements OnInit {
  loading = true;
  submitting = false;

  courseId: number | null = null;

  qcm: UserQcmDto | null = null;

  selected = new Map<number, Set<number>>();

  alert: { type: AlertType; message: string } | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private srvc: PublicSrvc
  ) {}

  ngOnInit(): void {
    const n = Number(this.route.snapshot.paramMap.get('courseId'));
    this.courseId = Number.isFinite(n) ? n : null;

    if (!this.courseId) {
      this.loading = false;
      this.alert = { type: 'danger', message: 'Identifiant du cours invalide.' };
      return;
    }

    this.loadQcm();
  }

  loadQcm(): void {
    this.loading = true;
    this.alert = null;

    this.srvc.getQcm(this.courseId!).subscribe({
      next: (res: UserQcmDto) => {
        this.qcm = res;

        this.selected.clear();
        for (const q of res.questions) this.selected.set(q.questionId, new Set<number>());

        this.loading = false;
      },
      error: (e: any) => {
        this.loading = false;
        const msg = (e?.error ?? '').toString();
        this.alert = { type: 'danger', message: msg || 'Impossible de charger le QCM.' };
      },
    });
  }

  toggleChoice(questionId: number, choixId: number, checked: boolean): void {
    const set = this.selected.get(questionId) ?? new Set<number>();
    if (checked) set.add(choixId);
    else set.delete(choixId);
    this.selected.set(questionId, set);
  }

  isChecked(questionId: number, choixId: number): boolean {
    return this.selected.get(questionId)?.has(choixId) ?? false;
  }

  submit(): void {
    if (!this.courseId || !this.qcm) return;
    if (this.submitting) return;

    const answers = this.qcm.questions.map((q) => ({
      questionId: q.questionId,
      selectedChoixIds: Array.from(this.selected.get(q.questionId) ?? []),
    }));

    const anySelected = answers.some((a) => (a.selectedChoixIds?.length ?? 0) > 0);
    if (!anySelected) {
      this.alert = { type: 'info', message: 'Veuillez sélectionner au moins une réponse.' };
      return;
    }

    this.submitting = true;
    this.alert = null;

    this.srvc.submitQcm(this.courseId, { answers }).subscribe({
      next: (res: UserQcmSubmitResultDto) => {
        this.submitting = false;

        const ok = res.statutTentative === 'REUSSI';
        const label = ok ? 'Réussi ✅' : 'Échoué ❌';

        this.alert = {
          type: ok ? 'success' : 'danger',
          message: `${label} — Note: ${res.noteSur20}/20`,
        };

        setTimeout(() => {
          this.router.navigate(['/public/mes-resultats']);
        }, 1500);
      },
      error: (e: any) => {
        this.submitting = false;
        const msg = (e?.error ?? '').toString();
        this.alert = { type: 'danger', message: msg || 'Soumission impossible.' };
      },
    });
  }

  back(): void {
    this.router.navigate(['/public/mes-inscriptions']);
  }
}
