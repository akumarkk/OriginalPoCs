import { z } from 'zod';

export const DeepDiveArticlePropsSchema = z.object({
  title: z.string().describe('The title of the article.'),
  content: z.array(z.string()).describe('The paragraphs of the article.'),
  author: z.string().describe('The author of the article.'),
});

export type DeepDiveArticleProps = z.infer<typeof DeepDiveArticlePropsSchema>;

export const DeepDiveArticle = ({ title, content, author }: DeepDiveArticleProps) => {
  return (
    <article className="prose lg:prose-xl mx-auto p-6 bg-gray-50 rounded-lg">
      <h1 className="text-3xl font-bold mb-4">{title}</h1>
      <div className="text-gray-700 leading-relaxed space-y-4">
        {content.map((paragraph, i) => <p key={i}>{paragraph}</p>)}
      </div>
      <footer className="mt-6 text-sm text-gray-400 italic">By {author}</footer>
    </article>
  );
};