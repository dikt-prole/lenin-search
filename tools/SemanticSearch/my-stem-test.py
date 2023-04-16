from pymystem3 import Mystem

mystem = Mystem()

text = 'Лондон это столица Великобритании'
lemmatized_text = mystem.lemmatize(text)

print(lemmatized_text)