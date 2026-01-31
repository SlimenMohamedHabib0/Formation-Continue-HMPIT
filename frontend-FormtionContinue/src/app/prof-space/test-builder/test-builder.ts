import { Component, OnDestroy, OnInit } from '@angular/core'
import { CommonModule } from '@angular/common'
import { FormsModule } from '@angular/forms'
import { ActivatedRoute, Router } from '@angular/router'
import { Subscription } from 'rxjs'
import { Ichoix } from '../interfaces/ichoix'
import { IQcmValidityDto, Iquestion, IquestionCreateDto, IquestionUpdateDto } from '../interfaces/iquestion'
import { ProfEvalSrvc } from '../services/prof-eval-srvc'

type Mode = 'list' | 'create' | 'edit'

@Component({
  selector: 'app-test-builder',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './test-builder.html',
  styleUrl: './test-builder.css',
})
export class TestBuilder implements OnInit, OnDestroy {
  courseId: number | null = null

  loading = true
  saving = false

  error: string | null = null
  success: string | null = null

  mode: Mode = 'list'

  questions: Iquestion[] = []
  validity: IQcmValidityDto | null = null

  editId: number | null = null

  enonce = ''
  points: number | null = null
  choix: Ichoix[] = []

  private sub = new Subscription()

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private evalSrvc: ProfEvalSrvc
  ) {}

  ngOnInit(): void {
    const idStr = this.route.snapshot.paramMap.get('courseId')
    const n = Number(idStr)
    this.courseId = Number.isFinite(n) ? n : null

    if (!this.courseId) {
      this.loading = false
      this.error = 'Identifiant du cours invalide.'
      return
    }

    this.loadAll()
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe()
  }

  back(): void {
    this.router.navigate(['/prof/mes-cours'])
  }

  resetMessages(): void {
    this.error = null
    this.success = null
  }

  private extractBackendMessage(e: any): string {
    const raw = e?.error

    if (typeof raw === 'string') return raw.trim()

    if (raw && typeof raw === 'object') {
      const m = raw?.message ?? raw?.Message ?? raw?.title ?? raw?.Title
      if (typeof m === 'string' && m.trim()) return m.trim()

      const errors = raw?.errors
      if (errors && typeof errors === 'object') {
        const firstKey = Object.keys(errors)[0]
        const arr = (errors as any)[firstKey]
        if (Array.isArray(arr) && arr[0]) return String(arr[0]).trim()
      }
    }

    return ''
  }

  private friendly(action: 'load' | 'save' | 'delete' | 'validity' | 'publish', e: any): string {
    const status = e?.status
    const msg = this.extractBackendMessage(e)
    const lower = msg.toLowerCase()

    if (status === 0) return 'Connexion interrompue. Réessayez.'
    if (status === 401) return 'Session expirée. Veuillez vous reconnecter.'
    if (status === 403) return 'Accès refusé.'
    if (status === 404) return 'Ressource introuvable.'

    if (lower.includes('cannot modify qcm') && lower.includes('attempts exist'))
      return 'Modification impossible : des tentatives existent déjà pour ce cours.'

    if (lower.includes('total points cannot exceed 20'))
      return 'Total des points > 20. Ajustez la pondération.'

    if (lower.includes('points must be > 0'))
      return 'Les points doivent être supérieurs à 0.'

    if (lower.includes('qcm invalid: total points'))
      return 'QCM invalide : le total doit être exactement 20 points.'

    if (lower.includes('has no correct choices') || lower.includes('no correct choices'))
      return 'QCM invalide : une question n’a aucune bonne réponse.'

    if (lower.includes('all choices are correct'))
      return 'QCM invalide : une question ne peut pas avoir tous les choix corrects.'

    if (lower.includes('duplicate choice'))
      return 'QCM invalide : une question contient des choix en double.'

    if (lower.includes('question already linked'))
      return 'Cette question est déjà liée à ce cours.'

    if (msg) return msg

    if (action === 'load') return 'Impossible de charger les questions.'
    if (action === 'validity') return 'Impossible de vérifier la validité du QCM.'
    if (action === 'delete') return 'Impossible de supprimer la question.'
    if (action === 'publish') return 'Impossible de publier. Vérifiez le PDF et le QCM.'
    return 'Action impossible.'
  }

  loadAll(): void {
    this.loading = true
    this.resetMessages()

    this.sub.add(
      this.evalSrvc.getQuestions(this.courseId!).subscribe({
        next: (res) => {
          this.questions = res ?? []
          this.loading = false
          this.loadValidity()
        },
        error: (e) => {
          this.questions = []
          this.loading = false
          this.error = this.friendly('load', e)
        },
      })
    )
  }

  loadValidity(): void {
    this.validity = null

    this.sub.add(
      this.evalSrvc.getQcmValidity(this.courseId!).subscribe({
        next: (res) => {
          this.validity = res ?? null
        },
        error: (e) => {
          this.validity = null
          this.error = this.friendly('validity', e)
        },
      })
    )
  }

  startCreate(): void {
    this.mode = 'create'
    this.editId = null
    this.enonce = ''
    this.points = null
    this.choix = [
      { libelle: '', estCorrect: false },
      { libelle: '', estCorrect: false },
    ]
    this.resetMessages()
  }

  startEdit(q: Iquestion): void {
    this.mode = 'edit'
    this.editId = q.id
    this.enonce = q.enonce ?? ''
    this.points = q.points ?? null
    this.choix = (q.choix ?? []).map((c) => ({
      libelle: c.libelle ?? '',
      estCorrect: !!c.estCorrect,
    }))
    while (this.choix.length < 2) this.choix.push({ libelle: '', estCorrect: false })
    this.resetMessages()
  }

  cancelForm(): void {
    this.mode = 'list'
    this.editId = null
    this.enonce = ''
    this.points = null
    this.choix = []
    this.resetMessages()
  }

  addChoice(): void {
    this.choix.push({ libelle: '', estCorrect: false })
  }

  removeChoice(i: number): void {
    if (this.choix.length <= 2) return
    this.choix.splice(i, 1)
  }

  private normalizePayload(): IquestionCreateDto | IquestionUpdateDto | null {
    const enonce = (this.enonce ?? '').trim()
    const p = this.points

    if (!enonce) return null
    if (p === null || p === undefined) return null

    const points = Number(p)
    if (!Number.isFinite(points) || points <= 0) return null

    const cleanedChoix = (this.choix ?? [])
      .map((c) => ({
        libelle: (c.libelle ?? '').trim(),
        estCorrect: !!c.estCorrect,
      }))
      .filter((c) => !!c.libelle)

    if (cleanedChoix.length < 2) return null

    return { enonce, points, choix: cleanedChoix }
  }

  submit(): void {
    if (!this.courseId) return
    if (this.saving) return

    this.resetMessages()

    const payload = this.normalizePayload()
    if (!payload) {
      this.error = 'Veuillez vérifier les champs.'
      return
    }

    const correctCount = payload.choix.filter((c) => c.estCorrect).length
    if (correctCount === 0) {
      this.error = 'Sélectionnez au moins une bonne réponse.'
      return
    }

    this.saving = true

    if (this.mode === 'create') {
      this.evalSrvc.addQuestion(this.courseId, payload).subscribe({
        next: () => {
          this.saving = false
          this.success = 'Question ajoutée.'
          this.cancelForm()
          this.loadAll()
        },
        error: (e) => {
          this.saving = false
          this.error = this.friendly('save', e)
        },
      })
      return
    }

    if (this.mode === 'edit' && this.editId) {
      this.evalSrvc.updateQuestion(this.courseId, this.editId, payload).subscribe({
        next: () => {
          this.saving = false
          this.success = 'Question mise à jour.'
          this.cancelForm()
          this.loadAll()
        },
        error: (e) => {
          this.saving = false
          this.error = this.friendly('save', e)
        },
      })
    }
  }

  deleteQuestion(q: Iquestion): void {
    if (!this.courseId) return
    if (this.saving) return

    if (!confirm('Supprimer cette question ?')) return

    this.resetMessages()
    this.saving = true

    this.evalSrvc.deleteQuestion(this.courseId, q.id).subscribe({
      next: () => {
        this.saving = false
        this.success = 'Question supprimée.'
        this.loadAll()
      },
      error: (e) => {
        this.saving = false
        this.error = this.friendly('delete', e)
      },
    })
  }

  publishQcmAndCourse(): void {
    if (!this.courseId) return
    if (this.saving) return

    if (!confirm('Publier le cours ? (PDF + QCM valide requis)')) return

    this.resetMessages()
    this.saving = true

    this.evalSrvc.publishQcm(this.courseId).subscribe({
      next: () => {
        this.saving = false
        this.success = 'Cours publié.'
        this.loadAll()
        this.router.navigate(['/prof/mes-cours'])
      },
      error: (e) => {
        this.saving = false
        this.error = this.friendly('publish', e)
      },
    })
  }

  get totalPointsLocal(): number {
    return (this.questions ?? []).reduce((sum, q) => sum + (q.points ?? 0), 0)
  }

  badgeValidityClass(): string {
    if (!this.validity) return 'badge bg-secondary-lt'
    return this.validity.isValid ? 'badge bg-green-lt' : 'badge bg-orange-lt'
  }
}
