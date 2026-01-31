export interface CourseCountItemDto {
    courseId: number;
    courseTitre: string;
    count: number;
  }
  
  export interface CourseSuccessItemDto {
    courseId: number;
    courseTitre: string;
    attempts: number;
    passed: number;
    failed: number;
    successRatePercent: number;
  }
  
  export interface ProfessorDashboardDto {
    nbMyCoursesTotal: number;
    nbMyCoursesDraft: number;
    nbMyCoursesPublished: number;
  
    nbEnrollmentsTotal: number;
    nbEnrollmentsPending: number;
    nbEnrollmentsAccepted: number;
    nbEnrollmentsRefused: number;
  
    nbAttemptsTotal: number;
    nbAttemptsPassed: number;
    nbAttemptsFailed: number;
    averageNote: number | null;
    successRatePercent: number;
  
    topCoursesByEnrollments: CourseCountItemDto[];
    successRateByCourse: CourseSuccessItemDto[];
  }
  